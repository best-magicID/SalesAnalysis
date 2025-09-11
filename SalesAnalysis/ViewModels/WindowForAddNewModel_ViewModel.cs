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
        /// Стоимость модели
        /// </summary>
        public double PriceModel
        {
            get => _PriceModel;
            set => SetProperty(ref _PriceModel, value);
        }
        public double _PriceModel;

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
            SaveModelCommand = new RaiseCommand(SaveModelCommand_Execute, SaveModelCommand_CanExecute);
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
        /// Выполнить команду, сохранить модель
        /// </summary>
        /// <param name="parameter"></param>
        private bool SaveModelCommand_CanExecute(object parameter)
        {
            return NameModel != string.Empty && PriceModel >= 0;
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
