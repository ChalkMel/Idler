using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(fileName = "OrderResponseTimeEffect", menuName = "Spirit/Effect/Order Response Time")]
    public class OrderResponseTimeEffect : SpiritEffect
    {
        [Header("Time Settings")] [SerializeField]
        private bool useMultiplier = true; // true: умножение, false: добавление

        [SerializeField] private float timeMultiplier = 1.5f; // увеличиваем на 50%
        [SerializeField] private float timeAdd = 15f; // или добавляем 15 секунд

        [Header("Which Timers to Affect")] [SerializeField]
        private bool affectResponseTimer = true; // время на принятие заказа

        [SerializeField] private bool affectRequestTimer = true; // время на приготовление

        public override void Apply(Credits credits, SpiritBuffManager buffManager, float multiplier)
        {
            var visitorSpawner = FindFirstObjectByType<VisitorSpawner>();
            if (visitorSpawner == null) return;

            float actualMultiplier = useMultiplier ? timeMultiplier * multiplier : 1f;
            float actualAdd = !useMultiplier ? timeAdd * multiplier : 0f;

            visitorSpawner.ExtendOrderTime(affectResponseTimer, affectRequestTimer, actualMultiplier, actualAdd);
        }

        public override void Remove(Credits credits, SpiritBuffManager buffManager, float multiplier)
        {
            var visitorSpawner = FindFirstObjectByType<VisitorSpawner>();
            if (visitorSpawner == null) return;

            float actualMultiplier = useMultiplier ? 1f / (timeMultiplier * multiplier) : 1f;
            float actualAdd = !useMultiplier ? -(timeAdd * multiplier) : 0f;

            visitorSpawner.ExtendOrderTime(affectResponseTimer, affectRequestTimer, actualMultiplier, actualAdd);
        }
    }
}