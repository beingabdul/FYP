using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HostalManagement.Models.viewmodels
{
    public class FoodListsVM
    {
        public int FoodListId { get; set; }
        public string MealName { get; set; }
        public string MealPrice { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int> WeekdayId { get; set; }
        public Nullable<int> MealTypeId { get; set; }
        public  string MealType { get; set; }
        public string  Weekday { get; set; }
    }
}