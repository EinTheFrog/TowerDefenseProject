using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Managers
{
    public class MoneyManager : MonoBehaviour
    {
        [SerializeField] private float moneyUpdateTime = 1f;
        [SerializeField] private int initialMoney = 1;
        [SerializeField] private int moneyChange = 1;
        [SerializeField] private Text moneyText = default;

        private int _money = default;

        public int Money
        {
            get => _money;
            set
            {
                _money = value;
                moneyText.text = _money.ToString();
            }
        }

        private float _timePassed = 0;

        private void Start()
        {
            Money = initialMoney;
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
