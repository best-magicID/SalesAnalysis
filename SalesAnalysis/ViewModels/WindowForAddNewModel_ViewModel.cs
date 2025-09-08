namespace SalesAnalysis.ViewModels
{
    public class WindowForAddNewModel_ViewModel : BaseViewModel
    {
        #region ПОЛЯ И СВОЙСТВА

        /// <summary>
        /// Название модели
        /// </summary>
        public string NameModel
        {
            get => _NameModel;
            set => SetProperty(ref _NameModel, value);
        }
        private string _NameModel = string.Empty;

        /// <summary>
        /// Цена модели
        /// </summary>
        public string PriceModel
        {
            get => _PriceModel;
            set => SetProperty(ref _PriceModel, value);
        }
        public string _PriceModel = string.Empty;

        /// <summary>
        /// Флаг, сохранение данных
        /// </summary>
        public bool IsSave { get; set; } = false;


        public event Action? RequestClose;

        public RaiseCommand? SaveModelCommand { get; set; }

        #endregion


        #region Конструктор

        public WindowForAddNewModel_ViewModel()
        {
            SaveModelCommand = new RaiseCommand(SaveModelCommand_Execute);
        }

        public WindowForAddNewModel_ViewModel(string nameModel, string priceModel)
        {
            this.NameModel = nameModel;
            this.PriceModel = priceModel;

            SaveModelCommand = new RaiseCommand(SaveModelCommand_Execute);
        }

        #endregion

        #region МЕТОДЫ

        /// <summary>
        /// Выполнить команду, сохранить модель
        /// </summary>
        /// <param name="parameter"></param>
        private void SaveModelCommand_Execute(object parameter)
        {
            IsSave = true;

            OnClose();
        }

        /// <summary>
        /// Закрытие окна
        /// </summary>
        public void OnClose()
        {
            RequestClose?.Invoke();
        }

        #endregion
    }
}
