
using SagaPatternMichael.Order.Repositories;

namespace SagaPatternMichael.Order.Outboxs
{
    public abstract class OutboxProcess<E> : IOutboxProcess<E> where E : class
    {
        public abstract Task OutboxProcessPack(E e);
    }
}
