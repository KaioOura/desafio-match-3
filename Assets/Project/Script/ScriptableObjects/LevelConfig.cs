using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Gameplay/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public const int MinimumSize = 3;

        [SerializeField] private int _width = 10;
        [SerializeField] private int _height = 10;
        
        [HideInInspector][SerializeField] private bool[] _deadCells;

        public int Width => _width;
        public int Height => _height;

        public bool IsDead(int x, int y)
        {
            if (!HasMask()) return false;
            if ((uint)x >= (uint)_width || (uint)y >= (uint)_height) return false;

            return _deadCells[y * _width + x];
        }

        public int DeadCellCount()
        {
            if (!HasMask()) return 0;

            int count = 0;
            for (int i = 0; i < _deadCells.Length; i++)
            {
                if (_deadCells[i]) count++;
            }

            return count;
        }
        
        public bool[,] CreateDeadMask()
        {
            if (DeadCellCount() == 0) return null;

            bool[,] mask = new bool[_width, _height];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    mask[x, y] = _deadCells[y * _width + x];
                }
            }

            return mask;
        }

        private bool HasMask()
        {
            return _deadCells != null && _deadCells.Length == _width * _height;
        }

#if UNITY_EDITOR
        public void Resize(int width, int height)
        {
            width = Mathf.Max(MinimumSize, width);
            height = Mathf.Max(MinimumSize, height);
            if (width == _width && height == _height && HasMask()) return;

            bool[] resized = new bool[width * height];

            int copiedWidth = Mathf.Min(width, _width);
            int copiedHeight = Mathf.Min(height, _height);
            if (HasMask())
            {
                for (int y = 0; y < copiedHeight; y++)
                {
                    for (int x = 0; x < copiedWidth; x++)
                    {
                        resized[y * width + x] = _deadCells[y * _width + x];
                    }
                }
            }

            _width = width;
            _height = height;
            _deadCells = resized;
        }

        public void SetDead(int x, int y, bool dead)
        {
            if ((uint)x >= (uint)_width || (uint)y >= (uint)_height) return;
            if (!HasMask()) _deadCells = new bool[_width * _height];

            _deadCells[y * _width + x] = dead;
        }

        public void SetAllDead(bool dead)
        {
            _deadCells = new bool[_width * _height];
            if (!dead) return;

            for (int i = 0; i < _deadCells.Length; i++)
            {
                _deadCells[i] = true;
            }
        }
#endif
    }
}
