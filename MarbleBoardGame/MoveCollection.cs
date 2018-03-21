using System.Collections;
using System.Collections.Generic;

namespace MarbleBoardGame
{
    public class MoveCollection : IList<Move>
    {
        private Board board;
        private Position position;
        private List<Move> moveList;

        /// <summary>
        /// Finds the index of a move in the collection
        /// </summary>
        /// <param name="item">Move to search for</param>
        public int IndexOf(Move item)
        {
            return moveList.IndexOf(item);
        }

        /// <summary>
        /// Inserts a move in the collection
        /// </summary>
        /// <param name="index">Index to insert at</param>
        /// <param name="item">Move to insert</param>
        public void Insert(int index, Move item)
        {
            moveList.Insert(index, item);
        }

        /// <summary>
        /// Removes a move from the collection at the specified index
        /// </summary>
        /// <param name="index">Index of move to remove</param>
        public void RemoveAt(int index)
        {
            moveList.RemoveAt(index);
        }

        /// <summary>
        /// Gets the move at the specified index
        /// </summary>
        /// <param name="index">Index to retrieve move from the collection</param>
        public Move this[int index]
        {
            get
            {
                return moveList[index];
            }
            set
            {
                moveList[index] = value;
            }
        }

        /// <summary>
        /// Adds a move to the collection if it does not already exist
        /// </summary>
        public void Add(Move item)
        {
            if (board.IsValid(position, item))
            {
                if (!Contains(item))
                {
                    moveList.Add(item);
                }
            }
        }

        /// <summary>
        /// Clears all moves in the collection
        /// </summary>
        public void Clear()
        {
            moveList.Clear();
        }

        /// <summary>
        /// Checks if a move exists in the collection
        /// </summary>
        /// <param name="item">Move to check</param>
        public bool Contains(Move item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (moveList[i].Equals(item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copys the collection of moves to an array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Array index to start copying at</param>
        public void CopyTo(Move[] array, int arrayIndex)
        {
            moveList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of moves in the collection
        /// </summary>
        public int Count
        {
            get { return moveList.Count; }
        }

        /// <summary>
        /// Is the move collection read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the move from the collection
        /// </summary>
        /// <param name="item">Move to remove</param>
        public bool Remove(Move item)
        {
            return moveList.Remove(item);
        }

        /// <summary>
        /// Gets the enumerator for enumerating moves
        /// </summary>
        public IEnumerator<Move> GetEnumerator()
        {
            return moveList.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for enumerating
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return moveList.GetEnumerator();
        }

        /// <summary>
        /// Creates a new collection of moves
        /// </summary>
        public MoveCollection(Board board, Position position)
        {
            this.board = board;
            this.position = position;
            moveList = new List<Move>();
        }
    }
}
