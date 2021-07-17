using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Managers
{
    public class MoneyManager : MonoBehaviour
    {
        [SerializeField] private float moneyUpdateTime = 1f;
        [SerializeField] private int initialMoney = 1;
        [SerializeField] private int moneyChange = 1;

        private int _money = default;
        private Text[] _moneyTexts = default;
        private string textTag = "MoneyTextTag";

        public int Money
        {
            get => _money;
            set
            {
                _money = value;
                foreach (var moneyText in _moneyTexts)
                {
                    moneyText.text = "Energy " + _money;
                }
            }
        }

        private float _timePassed = 0;

        private void OnEnable()
        {
            _money = initialMoney;
            _moneyTexts = GameObject.FindGameObjectsWithTag(textTag).Select(gameObj => gameObj.GetComponent<Text>())
                .ToArray();
        }

        private void Update()
        {
            _timePassed += Time.deltaTime;
            if (_timePassed < moneyUpdateTime) return;
            
            Money += moneyChange;
            _timePassed -= moneyUpdateTime;
        }
    
    
    }
}
