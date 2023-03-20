using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XCalendar.Core.Extensions;
using XCalendar.Core.Models;
using XCalendar.Core.Enums;



namespace DisApp24
{
    public class CalendarPageViewModel : BaseViewModel
    {
        #region Properties
        public Calendar<CalendarDay> Calendar { get; set; } = new Calendar<CalendarDay>()
        {
            SelectionType = SelectionType.Single,
            SelectionAction = SelectionAction.Add
        };



        public CalendarDay OutsideCalendarDay { get; set; } = new CalendarDay();
        #endregion

        #region Commands
        public ICommand NavigateCalendarCommand { get; set; }
        public ICommand ChangeDateSelectionCommand { get; set; }
        #endregion

        #region Constructors
        public CalendarPageViewModel()
        {
            ChangeDateSelectionCommand = new Command<DateTime>(ChangeDateSelection);
            NavigateCalendarCommand = new Command<int>(NavigateCalendar);
           
            Calendar.DaysUpdated += Calendar_DaysUpdated;
            Calendar.UpdateDay(OutsideCalendarDay, Calendar.NavigatedDate);
            
            
        }
        #endregion

        #region Methods
        public void NavigateCalendar(int amount)
        {
            if (Calendar.NavigatedDate.TryAddMonths(amount, out DateTime targetDate))
            {
                Calendar.Navigate(targetDate - Calendar.NavigatedDate);
            }
            else
            {
                Calendar.Navigate(amount > 0 ? TimeSpan.MaxValue : TimeSpan.MinValue);
            }
        }
        public void ChangeDateSelection(DateTime dateTime)
        {
            Calendar?.ChangeDateSelection(dateTime);
        }

        private void Calendar_DaysUpdated(object sender, EventArgs e)
        {
            Calendar.UpdateDay(OutsideCalendarDay, Calendar.NavigatedDate);
        }
        #endregion

    }
}
