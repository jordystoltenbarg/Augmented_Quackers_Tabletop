using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Mirror.Cloud.ListServerService
{
    public sealed class ListServerServerApi : ListServerBaseApi, IListServerServerApi
    {
        private const int cPingInterval = 20;
        private const int cMaxPingFails = 15;

        private ServerJson _currentServer;
        public ServerJson GetCurrentServer() { return _currentServer; }

        private string _serverId;

        private Coroutine _pingCoroutine;
        /// <summary>
        /// If the server has already been added
        /// </summary>
        private bool _added;
        /// <summary>
        /// if a request is currently sending
        /// </summary>
        private bool _sending;
        /// <summary>
        /// If an update request was recently sent
        /// </summary>
        private bool _skipNextPing;
        /// <summary>
        /// How many failed pings in a row
        /// </summary>
        private int _pingFails = 0;

        public bool ServerInList => _added;

        private readonly List<GameObject> _players = new List<GameObject>();

        public ListServerServerApi(ICoroutineRunner pRunner, IRequestCreator pRequestCreator) : base(pRunner, pRequestCreator)
        {
        }

        public void Shutdown()
        {
            stopPingCoroutine();
            if (_added)
                removeServerWithoutCoroutine();

            _added = false;
        }

        public void AddServer(ServerJson pServer)
        {
            if (_added) { Logger.LogWarning("AddServer called when server was already adding or added"); return; }
            bool valid = pServer.Validate();
            if (!valid) { return; }

            runner.StartCoroutine(addServer(pServer));
        }

        public void UpdateServer(int pNewPlayerCount)
        {
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); return; }

            _currentServer.playerCount = pNewPlayerCount;
            UpdateServer(_currentServer);
        }

        public void UpdateServer(string pDisplayName)
        {
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); return; }

            _currentServer.displayName = pDisplayName;
            UpdateServer(_currentServer);
        }

        public void UpdateServerCustomDataValue(string pCustomDataKey, string pCustomDataValue)
        {
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); return; }

            for (int i = 0; i < _currentServer.customData.Length; i++)
                if (_currentServer.customData[i].key == pCustomDataKey) { _currentServer.customData[i].value = pCustomDataValue; break; }

            UpdateServer(_currentServer);
        }

        public void UpdateServer(ServerJson pServer)
        {
            // TODO, use PartialServerJson as Arg Instead
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); return; }

            PartialServerJson partialServer = new PartialServerJson
            {
                displayName = pServer.displayName,
                playerCount = pServer.playerCount,
                maxPlayerCount = pServer.maxPlayerCount,
                customData = pServer.customData,
            };
            partialServer.Validate();

            runner.StartCoroutine(updateServer(partialServer));
        }

        public void RemoveServer()
        {
            if (!_added) { return; }

            if (string.IsNullOrEmpty(_serverId))
            {
                Logger.LogWarning("Can not remove server because serverId was empty");
                return;
            }

            stopPingCoroutine();
            runner.StartCoroutine(removeServer());
        }

        public int GetServerPlayerCount()
        {
            return _currentServer.playerCount;
        }

        public List<GameObject> GetPlayersInServer()
        {
            return _players;
        }

        private void stopPingCoroutine()
        {
            if (_pingCoroutine != null)
            {
                runner.StopCoroutine(_pingCoroutine);
                _pingCoroutine = null;
            }
        }

        private IEnumerator addServer(ServerJson pServer)
        {
            _added = true;
            _sending = true;
            _currentServer = pServer;

            UnityWebRequest request = requestCreator.Post("servers", _currentServer);
            yield return requestCreator.SendRequestEnumerator(request, onSuccess, onFail);
            _sending = false;

            void onSuccess(string pResponseBody)
            {
                CreatedIdJson created = JsonUtility.FromJson<CreatedIdJson>(pResponseBody);
                _serverId = created.id;

                // Start ping to keep server alive
                _pingCoroutine = runner.StartCoroutine(ping());
            }
            void onFail(string pResponseBody)
            {
                _added = false;
            }
        }

        private IEnumerator updateServer(PartialServerJson pServer)
        {
            // wait to not be sending
            while (_sending)
            {
                yield return new WaitForSeconds(1);
            }

            // We need to check added incase Update is called soon after Add, and add failed
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); yield break; }

            _sending = true;
            UnityWebRequest request = requestCreator.Patch("servers/" + _serverId, pServer);
            yield return requestCreator.SendRequestEnumerator(request, onSuccess);
            _sending = false;

            void onSuccess(string pResponseBody)
            {
                _skipNextPing = true;

                if (_pingCoroutine == null)
                {
                    _pingCoroutine = runner.StartCoroutine(ping());
                }
            }
        }

        /// <summary>
        /// Keeps server alive in database
        /// </summary>
        /// <returns></returns>
        private IEnumerator ping()
        {
            while (_pingFails <= cMaxPingFails)
            {
                yield return new WaitForSeconds(cPingInterval);
                if (_skipNextPing)
                {
                    _skipNextPing = false;
                    continue;
                }

                _sending = true;
                UnityWebRequest request = requestCreator.Patch("servers/" + _serverId, new EmptyJson());
                yield return requestCreator.SendRequestEnumerator(request, onSuccess, onFail);
                _sending = false;
            }

            Logger.LogWarning("Max ping fails reached, stoping to ping server");
            _pingCoroutine = null;


            void onSuccess(string pResponseBody)
            {
                _pingFails = 0;
            }
            void onFail(string pResponseBody)
            {
                _pingFails++;
            }
        }

        private IEnumerator removeServer()
        {
            _sending = true;
            UnityWebRequest request = requestCreator.Delete("servers/" + _serverId);
            yield return requestCreator.SendRequestEnumerator(request);
            _sending = false;

            _added = false;
        }

        private void removeServerWithoutCoroutine()
        {
            if (string.IsNullOrEmpty(_serverId))
            {
                Logger.LogWarning("Can not remove server becuase serverId was empty");
                return;
            }

            UnityWebRequest request = requestCreator.Delete("servers/" + _serverId);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            operation.completed += (pOp) =>
            {
                Logger.LogResponse(request);
            };
        }
    }
}
