namespace SagaPatternMichael.Order.Outboxs
{
    public interface IOutboxProcess<E> where E : class
    {
        Task OutboxProcessPack(E e);
    }
}
