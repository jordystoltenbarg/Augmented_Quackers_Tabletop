using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Mirror.Cloud.ListServerService
{
    public sealed class ListServerServerApi : ListServerBaseApi, IListServerServerApi
    {
        private const int PingInterval = 20;
        private const int MaxPingFails = 15;

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

        private List<GameObject> _players = new List<GameObject>();

        public ListServerServerApi(ICoroutineRunner runner, IRequestCreator requestCreator) : base(runner, requestCreator)
        {
        }

        public void Shutdown()
        {
            stopPingCoroutine();
            if (_added)
            {
                removeServerWithoutCoroutine();
            }
            _added = false;
        }

        public void AddServer(ServerJson server)
        {
            if (_added) { Logger.LogWarning("AddServer called when server was already adding or added"); return; }
            bool valid = server.Validate();
            if (!valid) { return; }

            runner.StartCoroutine(addServer(server));
        }

        public void UpdateServer(int newPlayerCount)
        {
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); return; }

            _currentServer.playerCount = newPlayerCount;
            UpdateServer(_currentServer);
        }

        public void UpdateServer(string pDisplayName)
        {
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); return; }

            _currentServer.displayName = pDisplayName;
            UpdateServer(_currentServer);
        }

        public void UpdateServer(ServerJson server)
        {
            // TODO, use PartialServerJson as Arg Instead
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); return; }

            PartialServerJson partialServer = new PartialServerJson
            {
                displayName = server.displayName,
                playerCount = server.playerCount,
                maxPlayerCount = server.maxPlayerCount,
                customData = server.customData,
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

        void stopPingCoroutine()
        {
            if (_pingCoroutine != null)
            {
                runner.StopCoroutine(_pingCoroutine);
                _pingCoroutine = null;
            }
        }

        IEnumerator addServer(ServerJson server)
        {
            _added = true;
            _sending = true;
            _currentServer = server;

            UnityWebRequest request = requestCreator.Post("servers", _currentServer);
            yield return requestCreator.SendRequestEnumerator(request, onSuccess, onFail);
            _sending = false;

            void onSuccess(string responseBody)
            {
                CreatedIdJson created = JsonUtility.FromJson<CreatedIdJson>(responseBody);
                _serverId = created.id;

                // Start ping to keep server alive
                _pingCoroutine = runner.StartCoroutine(ping());
            }
            void onFail(string responseBody)
            {
                _added = false;
            }
        }

        IEnumerator updateServer(PartialServerJson server)
        {
            // wait to not be sending
            while (_sending)
            {
                yield return new WaitForSeconds(1);
            }

            // We need to check added incase Update is called soon after Add, and add failed
            if (!_added) { Logger.LogWarning("UpdateServer called when before server was added"); yield break; }

            _sending = true;
            UnityWebRequest request = requestCreator.Patch("servers/" + _serverId, server);
            yield return requestCreator.SendRequestEnumerator(request, onSuccess);
            _sending = false;

            void onSuccess(string responseBody)
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
        IEnumerator ping()
        {
            while (_pingFails <= MaxPingFails)
            {
                yield return new WaitForSeconds(PingInterval);
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


            void onSuccess(string responseBody)
            {
                _pingFails = 0;
            }
            void onFail(string responseBody)
            {
                _pingFails++;
            }
        }

        IEnumerator removeServer()
        {
            _sending = true;
            UnityWebRequest request = requestCreator.Delete("servers/" + _serverId);
            yield return requestCreator.SendRequestEnumerator(request);
            _sending = false;

            _added = false;
        }

        void removeServerWithoutCoroutine()
        {
            if (string.IsNullOrEmpty(_serverId))
            {
                Logger.LogWarning("Can not remove server becuase serverId was empty");
                return;
            }

            UnityWebRequest request = requestCreator.Delete("servers/" + _serverId);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            operation.completed += (op) =>
            {
                Logger.LogResponse(request);
            };
        }
    }
}
