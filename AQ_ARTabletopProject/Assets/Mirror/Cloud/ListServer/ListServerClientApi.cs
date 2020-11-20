using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Mirror.Cloud.ListServerService
{
    public sealed class ListServerClientApi : ListServerBaseApi, IListServerClientApi
    {
        readonly ServerListEvent _onServerListUpdated;

        Coroutine _getServerListRepeatCoroutine;

        public event UnityAction<ServerCollectionJson> onServerListUpdated
        {
            add => _onServerListUpdated.AddListener(value);
            remove => _onServerListUpdated.RemoveListener(value);
        }

        public ListServerClientApi(ICoroutineRunner pRunner, IRequestCreator pRequestCreator, ServerListEvent pOnServerListUpdated) : base(pRunner, pRequestCreator)
        {
            _onServerListUpdated = pOnServerListUpdated;
        }

        public void Shutdown()
        {
            StopGetServerListRepeat();
        }

        public void GetServerList()
        {
            runner.StartCoroutine(getServerList());
        }

        public void StartGetServerListRepeat(int pInterval)
        {
            _getServerListRepeatCoroutine = runner.StartCoroutine(GetServerListRepeat(pInterval));
        }

        public void StopGetServerListRepeat()
        {
            // if runner is null it has been destroyed and will alraedy be null
            if (runner.IsNotNull() && _getServerListRepeatCoroutine != null)
            {
                runner.StopCoroutine(_getServerListRepeatCoroutine);
            }
        }

        IEnumerator GetServerListRepeat(int pInterval)
        {
            while (true)
            {
                yield return getServerList();

                yield return new WaitForSeconds(pInterval);
            }
        }
        IEnumerator getServerList()
        {
            UnityWebRequest request = requestCreator.Get("servers");
            yield return requestCreator.SendRequestEnumerator(request, onSuccess);

            void onSuccess(string pResponseBody)
            {
                ServerCollectionJson serverlist = JsonUtility.FromJson<ServerCollectionJson>(pResponseBody);
                _onServerListUpdated?.Invoke(serverlist);
            }
        }
    }
}
