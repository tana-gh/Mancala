using System;
using System.Collections.Generic;
using System.Linq;

namespace tana_gh.Mancala
{
    [Role("EntityStore", SceneKind.Game, typeof(Game))]
    [Role("EntityStore", SceneKind.Game, typeof(Player))]
    [Role("EntityStore", SceneKind.Game, typeof(Board))]
    [Role("EntityStore", SceneKind.Game, typeof(Pocket))]
    [Role("EntityStore", SceneKind.Game, typeof(Stone))]
    public class EntityStore<T> where T : IEntity
    {
        private List<T> List { get; } = new();
        private Dictionary<Guid, T> Dictionary { get; } = new();

        public int Count => List.Count;

        public void Clear()
        {
            List.Clear();
            Dictionary.Clear();
        }

        public void Add(T entity)
        {
            List.Add(entity);
            Dictionary.Add(entity.Id, entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public void Remove(T entity)
        {
            List.Remove(entity);
            Dictionary.Remove(entity.Id);
        }

        public T Get(Guid id)
        {
            return Dictionary[id];
        }

        public T GetSingle()
        {
            return List[0];
        }

        public T GetNext(T entity)
        {
            return List[(IndexOf(entity) + 1) % List.Count];
        }

        public IEnumerable<T> GetAll()
        {
            return List;
        }

        public T ElementAt(int index)
        {
            return List[index];
        }

        public int IndexOf(T entity)
        {
            return List.IndexOf(entity);
        }
    }
}
