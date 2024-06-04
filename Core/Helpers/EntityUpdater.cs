namespace WebApi.Helpers
{
    public static class EntityUpdater
    {
        public static void UpdateIfNotNull<T>(T target, Action<T> updateAction) where T : class
        {
            if (target != null)
            {
                updateAction(target);
            }
        }

        public static void UpdateIfNotNull<T>(T? target, Action<T> updateAction) where T : struct
        {
            if (target.HasValue)
            {
                updateAction(target.Value);
            }
        }
    }
}
