using System;
using StatsManager.StatsTypes;
using Utils;

namespace StatsManager
{
    public class StatModifier : IDisposable
    {
        public event Action<StatModifier> OnDispose = delegate { };
    
        private readonly CountdownTimer _timer;
        private readonly StatType _type;
        private readonly Func<float, float> _operation;
        private readonly IHasStatModifier _source;
        public bool MarkedForRemoval { get; private set; }
        public IHasStatModifier Source => _source;

        public StatModifier(StatType type, float duration, Func<float, float> operation, IHasStatModifier source) {
            _type = type;
            _operation = operation;
            _source = source;

            // если duration <= 0, значит модификатор постоянный
            if (duration <= 0) return;

            // ? переписать на корутины?
            _timer = new CountdownTimer(duration);
            _timer.OnTimerStop += () => MarkedForRemoval = true;  // когда таймер кончается, вызывается метод dispose для удаления модификатора
            _timer.Start();
        }

        public void Update(float deltaTime) => _timer?.Tick(deltaTime);

        // берем значение из Query и применяем операцию к стату
        public void Handle(object sender, Query query) {
            if (query.StatType == _type) {
                query.Value = _operation(query.Value);
            }
        }

        // удаление модификатора из списка когда истечет таймер
        public void Dispose() {
            OnDispose.Invoke(this);
        }
    }
}