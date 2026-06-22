using System.Collections.Generic;
using UnityEngine;

namespace Tidz
{
    public class LocalMonoPool<TComp> where TComp : Component
    {
        private readonly Queue<TComp> _pool = new();
        private int _created;

        public TComp prefab { get; private set; }
        public Transform parent { get; private set; }

        public int CountInactive => _pool.Count;
        public int CountCreated => _created;

        public void Configure(TComp prefab, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var it = Create();
                Release(it);
            }
        }

        public TComp Get()
        {
            if (_pool.Count > 0)
            {
                var it = _pool.Dequeue();
                OnGet(it);
                return it;
            }

            var created = Create();
            OnGet(created);
            return created;
        }

        public void Release(TComp item)
        {
            if (!item) return;
            OnRelease(item);
            _pool.Enqueue(item);
        }

        public void Clear()
        {
            while (_pool.Count > 0)
            {
                var it = _pool.Dequeue();
                if (it) Object.Destroy(it.gameObject);
            }
            _created = 0;
        }

        private TComp Create()
        {
            if (!prefab) throw new System.InvalidOperationException($"[{GetType().Name}] Prefab chưa được cấu hình.");

            var inst = Object.Instantiate(prefab, parent);
            inst.gameObject.SetActive(false);
            _created++;
            return inst;
        }

        protected virtual void OnGet(TComp item)
        {
            if (item) item.gameObject.SetActive(true);
        }

        protected virtual void OnRelease(TComp item)
        {
            if (item) item.gameObject.SetActive(false);
        }
    }
}
