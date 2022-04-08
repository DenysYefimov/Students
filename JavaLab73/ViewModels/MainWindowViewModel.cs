using System.ComponentModel;
using System.Collections.ObjectModel;
using System;
using JavaLab73.Models;
using JavaLab73.ViewModels.Base;
using System.Windows.Data;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using JavaLab73.Infrastructure.Commands;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;

namespace JavaLab73.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            Students = _lst;
            _selectedStudents.Filter += OnStudentFilteredByName;
            _selectedStudents.Filter += OnStudentFilteredByGpa;
            foreach (var student in _students)
            {
                if (_temp.ContainsKey(student.Birthday.Year.ToString())) ++_temp[student.Birthday.Year.ToString()];
                else _temp.Add(student.Birthday.Year.ToString(), 1);
            }
            AddStudentCommand = new RelayCommand(OnAddStudentCommandExecuted, CanAddStudentCommandExecute);
            RemoveStudedentCommand = new RelayCommand(OnRemoveStudedentCommandExecuted, CanRemoveStudedentCommandExecute);
            WriteStudentsListToFileCommand = new RelayCommand(OnWriteStudentsListToFileCommandExecuted);
            ReadStudentsListFromFileCommand = new RelayCommand(OnWReadStudentsListFromFileCommandExecuted);
        }

        #region Вибрані студенти

        /// <summary> Вибрані студенти </summary>
        private CollectionViewSource _selectedStudents = new CollectionViewSource();

        /// <summary> Вибрані студенти </summary>
        public ICollectionView SelectedStudents => _selectedStudents?.View;

        private void OnStudentFilteredByName(object sender, FilterEventArgs e)
        {
            if (e.Item is not Student student)
            {
                e.Accepted = false;
                return;
            }

            var filterText = _studentFilterByNameText;
            if (string.IsNullOrEmpty(filterText) || student.FirstName.Contains(filterText, StringComparison.OrdinalIgnoreCase))
                return;

            e.Accepted = false;
        }

        private void OnStudentFilteredByGpa(object sender, FilterEventArgs e)
        {
            if (e.Item is not Student student)
            {
                e.Accepted = false;
                return;
            }

            var filterText = _studentFilterByGpaText;
            if (string.IsNullOrEmpty(filterText) || !double.TryParse(filterText, out double result) || student.GPA > result)
                return;

            e.Accepted = false;
        }

        #endregion

        #region Список студентів

        private ObservableCollection<Student> _lst = new ObservableCollection<Student>()
        {
            new Student("Василенко", "Василь", "Васильович", new DateTime(2003, 01, 03), "Миколаївська область, Новоодеський район, м. Нова Одеса, вул. Кухарєва 7", "+380 (68) 439 81 69", 95.95),
            new Student("Іванов", "Іван", "Іванович", new DateTime(2002, 05, 17), "м. Миколаїв, просп. Центральний 13", "+380 (95) 563 21 13", 66.66),
            new Student("Петров", "Петро", "Петрович", new DateTime(2000, 07, 23), "м. Миколаїв, просп. Миру 127", "+380 (65) 290 22 41", 82.31),
            new Student("Васильов", "Антон", "Васильович", new DateTime(2001, 02, 14), "м. Миколаїв, вул. Соборна 15", "+380 (97) 756 00 29", 98.93),
            new Student("Іваненко", "Микита", "Іванович", new DateTime(2001, 11, 06), "м. Миколаїв, вул. Театральна 46", "+380 (50) 497 11 75", 63.72),
            new Student("Петренко", "Артем", "Петрович", new DateTime(2000, 04, 30), "м. Миколаїв, вул. Садова 123", "+380 (73) 920 44 55", 74.49),

            new Student("Василенко", "Богдан", "Васильович", new DateTime(2004, 12, 23), "Миколаївська область, Новоодеський район, м. Нова Одеса, вул. Кухарєва 7", "+380 (68) 439 81 69", 78.93),
            new Student("Іванов", "Сергій", "Іванович", new DateTime(2002, 06, 28), "м. Миколаїв, просп. Центральний 13", "+380 (95) 563 21 13", 84.43),
            new Student("Петров", "Микола", "Петрович", new DateTime(2003, 07, 23), "м. Миколаїв, просп. Миру 127", "+380 (65) 290 22 41", 99.32),
            new Student("Васильов", "Олександр", "Васильович", new DateTime(2001, 04, 07), "м. Миколаїв, вул. Соборна 15", "+380 (97) 756 00 29", 77.31),
            new Student("Іваненко", "Валентин", "Іванович", new DateTime(2000, 12, 31), "м. Миколаїв, вул. Театральна 46", "+380 (50) 497 11 75", 69.19),
            new Student("Петренко", "Євген", "Петрович", new DateTime(2001, 01, 01), "м. Миколаїв, вул. Садова 123", "+380 (73) 920 44 55", 61.54),

            new Student("Антонов", "Максим", "Васильович", new DateTime(2003, 02, 27), "Миколаївська область, Новоодеський район, м. Нова Одеса, вул. Кухарєва 7", "+380 (68) 439 81 69", 92.47),
            new Student("Кандиба", "Тарас", "Іванович", new DateTime(2004, 08, 13), "м. Миколаїв, просп. Центральний 13", "+380 (95) 563 21 13", 78.58),
            new Student("М'ясоєдов", "Ілля", "Петрович", new DateTime(2002, 09, 25), "м. Миколаїв, просп. Миру 127", "+380 (65) 290 22 41", 67.14),
            new Student("Жиголін", "Вадим", "Васильович", new DateTime(2001, 10, 15), "м. Миколаїв, вул. Соборна 15", "+380 (97) 756 00 29", 96.69),
            new Student("Крутивус", "Владислав", "Іванович", new DateTime(2003, 03, 08), "м. Миколаїв, вул. Театральна 46", "+380 (50) 497 11 75", 69.96),
            new Student("Федченко", "Руслан", "Петрович", new DateTime(2000, 06, 17), "м. Миколаїв, вул. Садова 123", "+380 (73) 920 44 55", 85.73),

            new Student("Вернидуб", "Олег", "Васильович", new DateTime(2004, 06, 06), "Миколаївська область, Новоодеський район, м. Нова Одеса, вул. Кухарєва 7", "+380 (68) 439 81 69", 97.64),
            new Student("Потапенко", "Ростислав", "Іванович", new DateTime(2002, 05, 24), "м. Миколаїв, просп. Центральний 13", "+380 (95) 563 21 13", 60.02),
            new Student("Брюховець", "Павло", "Петрович", new DateTime(2000, 02, 11), "м. Миколаїв, просп. Миру 127", "+380 (65) 290 22 41", 100),
            new Student("Вернидуб", "Любомир", "Васильович", new DateTime(2001, 04, 02), "м. Миколаїв, вул. Соборна 15", "+380 (97) 756 00 29", 83.12),
            new Student("М'ясоєдов", "Михайло", "Іванович", new DateTime(2001, 12, 22), "м. Миколаїв, вул. Театральна 46", "+380 (50) 497 11 75", 75.21),
            new Student("Жиголін", "Андрій", "Петрович", new DateTime(2000, 01, 15), "м. Миколаїв, вул. Садова 123", "+380 (73) 920 44 55", 99.99)
        };

        /// <summary> Список студентів </summary>
        private ObservableCollection<Student> _students = new ObservableCollection<Student>();

        /// <summary> Список студентів </summary>
        public ObservableCollection<Student> Students
        {
            get => _students;
            set
            {
                if (!Set(ref _students, value)) return;
                _selectedStudents.Source = value;
                OnPropertyChanged(nameof(SelectedStudents));
            }
        }

        #endregion

        #region Роки народження студентів

        /// <summary> Роки народження студентів </summary>
        private SortedDictionary<string, int> _temp = new SortedDictionary<string, int>();

        /// <summary> Роки народження студентів </summary>
        public SortedDictionary<string, int> Temp
        {
            get => _temp;
            set
            {
                Set(ref _temp, value);
                OnPropertyChanged(nameof(Years));
            }
        }

        /// <summary> Роки народження студентів </summary>
        private ObservableCollection<KeyValuePair<string, int>> _years = new ObservableCollection<KeyValuePair<string, int>>();

        /// <summary> Роки народження студентів </summary>
        public ObservableCollection<KeyValuePair<string, int>> Years
        {
            get
            {
                _years.Clear();
                foreach (var pair in _temp)
                    _years.Add(pair);
                return _years;
            }
            set => Set(ref _years, value);
        }

        #endregion

        #region Вибраний студент

        /// <summary> Вибраний студент </summary>
        private Student _selectedStudent;

        /// <summary> Вибраний студент </summary>
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set => Set(ref _selectedStudent, value);
        }

        #endregion

        #region Тексти фільтрів студентів
        
        /// <summary> Текст фільтра студентів за ім'ям </summary>
        private string _studentFilterByNameText;

        /// <summary> Текст фільтра студентів за ім'ям </summary>
        public string StudentFilterByNameText
        {
            get => _studentFilterByNameText;
            set
            {
                if (!Set(ref _studentFilterByNameText, value)) return;
                _selectedStudents.View.Refresh();
            }
        }

        /// <summary> Текст фільтра студентів за середнім балом більшим, ніж </summary>
        private string _studentFilterByGpaText;

        /// <summary> Текст фільтра студентів за середнім балом більшим, ніж </summary>
        public string StudentFilterByGpaText
        {
            get => _studentFilterByGpaText;
            set
            {
                if (!Set(ref _studentFilterByGpaText, value)) return;
                _selectedStudents.View.Refresh();
            }
        }

        /// <summary> Текст фільтра студентів за кількістю студентів із максимальним середнім балом </summary>
        private string _studentFilterByAmountOfMaxGpaText;

        /// <summary> Текст фільтра студентів за кількістю студентів із максимальним середнім балом </summary>
        public string StudentFilterByAmountOfMaxGpaText
        {
            get => _studentFilterByAmountOfMaxGpaText;
            set
            {
                if (!(int.TryParse(value, out int result) & Set(ref _studentFilterByAmountOfMaxGpaText, value)));
                else
                {
                    SortedByGpaDescending = (from student in _students
                                             orderby student.GPA descending
                                             select student).ToArray();
                    Student[] temp = new Student[result];
                    Array.Copy(SortedByGpaDescending, temp, result);
                    _selectedStudents.Source = temp;
                    OnPropertyChanged(nameof(SelectedStudents));
                    _selectedStudents.View.Refresh();
                }
                if (string.IsNullOrEmpty(_studentFilterByAmountOfMaxGpaText))
                {
                    _selectedStudents.Source = Students;
                    OnPropertyChanged(nameof(SelectedStudents));
                }
            }
        }
        private Student[] SortedByGpaDescending;

        #endregion

        #region Команди

        #region Додавання студента

        private string _newStudentLastName;
        public string NewStudentLastName { get => _newStudentLastName; set => Set(ref _newStudentLastName, value); }

        private string _newStudentFirstName;
        public string NewStudentFirstName { get => _newStudentFirstName; set => Set(ref _newStudentFirstName, value); }

        private string _newStudentPatronymic;
        public string NewStudentPatronymic { get => _newStudentPatronymic; set => Set(ref _newStudentPatronymic, value); }

        private DateTime _newStudentBirthday;
        public DateTime NewStudentBirthday { get => _newStudentBirthday; set => Set(ref _newStudentBirthday, value); }

        private string _newStudentAddress;
        public string NewStudentAddress { get => _newStudentAddress; set => Set(ref _newStudentAddress, value); }

        private string _newStudentPhoneNumber;
        public string NewStudentPhoneNumber { get => _newStudentPhoneNumber; set => Set(ref _newStudentPhoneNumber, value); }

        private double _newStudentGpa;
        public double NewStudentGpa { get => _newStudentGpa; set => Set(ref _newStudentGpa, value); }

        public ICommand AddStudentCommand { get; }

        private void OnAddStudentCommandExecuted(object parameter)
        {
            _students.Add(new Student(NewStudentLastName, NewStudentFirstName, NewStudentPatronymic, NewStudentBirthday, NewStudentAddress, NewStudentPhoneNumber, NewStudentGpa));
            OnPropertyChanged(nameof(Students));
            if (Temp.ContainsKey(NewStudentBirthday.Year.ToString()))
                ++Temp[NewStudentBirthday.Year.ToString()];
            else 
                Temp.Add(NewStudentBirthday.Year.ToString(), 1);
            OnPropertyChanged(nameof(Years));
            NewStudentLastName = NewStudentFirstName = NewStudentPatronymic = NewStudentAddress = NewStudentPhoneNumber = default;
            NewStudentBirthday = default;
            NewStudentGpa = default;
        }

        private bool CanAddStudentCommandExecute(object parameter) => NewStudentLastName is not null && NewStudentFirstName is not null && NewStudentPatronymic is not null;

        #endregion

        #region Видалення студента

        public ICommand  RemoveStudedentCommand { get; }

        private void OnRemoveStudedentCommandExecuted(object parameter)
        {
            --Temp[SelectedStudent.Birthday.Year.ToString()];
            OnPropertyChanged(nameof(Years));
            Students.Remove(SelectedStudent);
        }

        private bool CanRemoveStudedentCommandExecute(object parameter) => SelectedStudent is not null;

        #endregion

        #region Запис списку студентів до файлу

        public ICommand WriteStudentsListToFileCommand { get; }

        private void OnWriteStudentsListToFileCommandExecuted(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            XmlSerializer xmlFormatter = new XmlSerializer(typeof(ObservableCollection<Student>));
            if (openFileDialog.ShowDialog() == true)
            {
                using Stream writer = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                xmlFormatter.Serialize(writer, Students);
            }
        }

        #endregion

        #region Зчитування списку студентів із файлу

        public ICommand ReadStudentsListFromFileCommand { get; }

        private void OnWReadStudentsListFromFileCommandExecuted(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                XmlSerializer xmlFormatter = new XmlSerializer(typeof(ObservableCollection<Student>));
                using (Stream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    Students = (ObservableCollection<Student>)xmlFormatter.Deserialize(reader);

                _temp.Clear();
                foreach (var student in _students)
                {
                    if (_temp.ContainsKey(student.Birthday.Year.ToString())) ++_temp[student.Birthday.Year.ToString()];
                    else _temp.Add(student.Birthday.Year.ToString(), 1);
                }
                OnPropertyChanged(nameof(Years));
            }
        }

        #endregion

        #endregion
    }
}