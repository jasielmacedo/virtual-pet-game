using UnityEngine;
using Core.Game;
using Game.Entities;
using System.Collections.Generic;

namespace Game.Manager
{
    public class OfficeInstance : Singleton<OfficeInstance>
    {
        public enum EPlaceCategory
        {
            Toilets,
            Coffee,
            WorkStation,
            Relax,
            Lunch,
            Water
        }

        public Dictionary<EPlaceCategory, Stack<Place>> availablePlaces = new Dictionary<EPlaceCategory, Stack<Place>>();

        private void OnEnable()
        {
            if (!SetInstance(this))
            {
                Destroy(this);
                return;
            }
        }

        public int TotalToiletsAvailable => availablePlaces.ContainsKey(EPlaceCategory.Toilets) ? availablePlaces[EPlaceCategory.Toilets].Count : 0;
        public int TotalCoffeePlacesAvailable => availablePlaces.ContainsKey(EPlaceCategory.Coffee) ? availablePlaces[EPlaceCategory.Coffee].Count : 0;
        public int TotalWorkstationsAvailable => availablePlaces.ContainsKey(EPlaceCategory.WorkStation) ? availablePlaces[EPlaceCategory.WorkStation].Count : 0;
        public int TotalPlacesToRelaxAvailable => availablePlaces.ContainsKey(EPlaceCategory.Relax) ? availablePlaces[EPlaceCategory.Relax].Count : 0;
        public int TotalLunchPlacesAvailable => availablePlaces.ContainsKey(EPlaceCategory.Lunch) ? availablePlaces[EPlaceCategory.Lunch].Count : 0;
        public int TotalWaterPlacesAvailable => availablePlaces.ContainsKey(EPlaceCategory.Water) ? availablePlaces[EPlaceCategory.Water].Count : 0;


        [SerializeField] private int startWorkHour = 9;
        [SerializeField] private int endWorkHour = 19;
        [SerializeField] private Place entrance;
        public Place Entrance => entrance;


        public bool IsWorkingHour => TimerManager.hour >= startWorkHour && TimerManager.hour < endWorkHour;

        public Place getPlaceAvailable(EPlaceCategory category)
        {
            if (availablePlaces.ContainsKey(category) && availablePlaces[category].Count > 0)
                return availablePlaces[category].Pop();
            return null;
        }

        public void setPlaceAvailable(EPlaceCategory category, Place place)
        {
            if (!availablePlaces.ContainsKey(category))
                availablePlaces.Add(category, new Stack<Place>());

            if (!availablePlaces[category].Contains(place))
                availablePlaces[category].Push(place);
        }
    }
}