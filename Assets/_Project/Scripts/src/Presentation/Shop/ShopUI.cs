using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Froggi.Game;

namespace Froggi.Presentation
{
	public class ShopUI : MonoBehaviour
	{
		[SerializeField] private float _itemSpacing = 0.5f;

		[SerializeField] private Transform _shopItemsContainer;

		[SerializeField] private GameObject _shopItemPrefab;

		[SerializeField] private ParticleSystem _purchaseFX;

		[SerializeField] private Transform _purchaseFXPos;

		[SerializeField] private TMP_Text _notEnoughCoinText;

		private List<ActorData> _actorsData;
		private List<ShopItemUI> _shopItems;

		private Shop _shop;
		private Wallet _wallet;

		private int _newSelectedItemIndex = 0;
		private int _previousSelectedItemIndex = 0;

		private float _itemHeight;

		public void Construct(Shop shop, Wallet wallet)
		{
			_shop = shop;
			_wallet = wallet;
			_purchaseFX.transform.position = _purchaseFXPos.position;
		}

		public void InitializeShopItems(List<Sprite> avatars, List<ActorData> actorsData, int selectedIndex = 0)
		{
			_actorsData ??= new List<ActorData>(actorsData);
			_shopItems ??= new List<ShopItemUI>();

			if (_shopItems.Count > 0)
				_shopItems.Clear();

			_itemHeight = _shopItemPrefab.GetComponent<RectTransform>().sizeDelta.y;

			var count = actorsData.Count;

			for (var i = 0; i < count; i++)
			{
				var character = actorsData[i];

				var shopItem = Instantiate(
					_shopItemPrefab,
					_shopItemsContainer).GetComponent<ShopItemUI>();

				shopItem.gameObject.name = "Item" + i + "-" + character.Name;

				shopItem.Initialize(
					i,
					avatars[i],
					actorsData[i],
					OnItemSelect,
					OnItemPurchase
				);

				shopItem.SetPosition(Vector2.down * i * (_itemHeight + _itemSpacing));

				if (i == selectedIndex)
				{
					shopItem.IsSelected = true;
				}

				_shopItems.Add(shopItem);
			}

			_shopItemsContainer.GetComponent<RectTransform>().sizeDelta =
				Vector2.up * (((_itemHeight + _itemSpacing) * count) + _itemSpacing);
		}

		private void OnItemSelect(int index)
		{
			_previousSelectedItemIndex = _newSelectedItemIndex;
			_newSelectedItemIndex = index;

			var previousItem = _shopItems[_previousSelectedItemIndex];
			var newItem = _shopItems[_newSelectedItemIndex];

			var actor = _actorsData[index];

			if (!_shop.Select(actor))
				return;

			previousItem.IsSelected = false;
			newItem.IsSelected = true;
		}

		private void OnItemPurchase(int index)
		{
			var shopItem = _shopItems[index];

			var actor = _actorsData[index];

			if (!_wallet.CanDraw(actor.Price))
			{
				ShowNotEnoughCoinMessage();
				shopItem.PlayItemShakeAnimation();
				return;
			}

			if (!_shop.Purchase(actor))
				return;

			_wallet.Draw(actor.Price);

			shopItem.IsPurchased = true;

			_purchaseFX.Play();
		}

		private void ShowNotEnoughCoinMessage()
		{
			_notEnoughCoinText.DOComplete();
			_notEnoughCoinText.transform.DOComplete();

			_notEnoughCoinText.DOFade(1f, 2.5f).From(0f).OnComplete(() => _notEnoughCoinText.DOFade(0f, 1f));

			_notEnoughCoinText.transform.DOShakePosition(3f, new Vector3(5f, 0f, 0f), 10, 0f);
		}
	}
}
