using HostalManagement.Models;
using System.Collections.Generic;

namespace HostalManagement.Controllers
{
    public class PlaceOrder
    {
        public WeekdayVM Weekday { get; set; }
        public List<MealTypeVM> MealType { get; set; }
        public List<FoodListVM> FoodList { get; set; }

    }
    public class WeekdayVM
    {
        public int WeekdayId { get; set; }
        public string Name { get; set; }
    }
    public class MealTypeVM
    {
        public int MealTypeId { get; set; }
        public string Name { get; set; }
    }
    public class FoodListVM
    {
        public int FoodListId { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int MealTypeId { get; set; }
    }
}