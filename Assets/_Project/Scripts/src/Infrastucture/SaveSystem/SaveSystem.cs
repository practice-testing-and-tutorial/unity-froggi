using Froggi.Game;

namespace Froggi.Infrastructure
{
	public class SaveSystem : ISaveSystem
	{
		private const string WalletFile = "wallet-data.txt";
		private const string ShopFile = "shop-data.txt";

		public ShopData GetShopData()
		{
			return BinarySerializer.Load<ShopData>(ShopFile);
		}

		public WalletData GetWalletData()
		{
			return BinarySerializer.Load<WalletData>(WalletFile);
		}

		public void SaveShopData(ShopData shopData)
		{
			BinarySerializer.Save<ShopData>(shopData, ShopFile);
		}

		public void SaveWalletData(WalletData walletData)
		{
			BinarySerializer.Save<WalletData>(walletData, WalletFile);
		}
	}
}
