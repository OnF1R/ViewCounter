namespace ViewCounter.Domain.Params
{
    public class PaginationParams
    {
        /// <summary>
        /// Максимальное количество элементов на странице
        /// </summary>
        private const int _maxItemPerPage = 50;
        /// <summary>
        /// Текущее количество элементов на странице
        /// </summary>
        private int itemsPerPage;
        /// <summary>
        /// Текущая страница
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// Свойство текущего количества элементов на странице
        /// </summary>
        public int ItemsPerPage
        {
            get => itemsPerPage;
            set => itemsPerPage = value > _maxItemPerPage ? _maxItemPerPage : value;
        }
    }
}
