using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JavaLab73.Models
{
    [Serializable]
    public class Student : INotifyPropertyChanged
    {
        public Student() { }
        public Student(string lastName, string firstName, string patronymic, DateTime birthday = default, string address = default, string phoneNumber = default, double gpa = default)
        {
            Id = _count++;
            LastName = lastName;
            FirstName = firstName;
            Patronymic = patronymic;
            Birthday = birthday;
            Address = address;
            PhoneNumber = phoneNumber;
            GPA = gpa;
        }
        public bool IsChanged { get; set; }
        [Required]
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string FirstName { get; set; }
        [Required, StringLength(50)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string Patronymic { get; set; }
        public DateTime Birthday { get; set; }
        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        public double GPA { get; set; }
        public string Sorting { get => LastName + FirstName; }
        private static int _count = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName != nameof(IsChanged))
            {
                IsChanged = true;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
