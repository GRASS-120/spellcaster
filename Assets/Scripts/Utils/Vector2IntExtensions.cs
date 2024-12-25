using UnityEngine;

// по первому аргументу метода класс определяет, какой класс/структуру нужно расширить
namespace Utils
{
    public static class Vector2IntExtensions {
        public static void Deconstruct(this Vector2Int vector, out int x, out int y) {
            x = vector.x;
            y = vector.y;
        }
    }
}
