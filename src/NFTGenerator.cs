using Godot;
using System;
using System.Collections.Generic;

public class NFTGenerator : Control
{
	private const string asset_path = "res://assets/Fake nfts/";
	public static bool DONE_LOADING = false;

	[Export]
	private Viewport view;

	/// <summary>
	/// List of outfit <paramref name="TextureRect"/> nodes.
	/// <para> 0 == Background </para>
	/// <para> 1 == Pattern </para>
	/// <para> 2 == Eyes </para>
	/// <para> 3 == Mouth </para>
	/// <para> 4 == Hat </para>
	/// <para> 5 == Acc. </para>
	/// </summary>
	private TextureRect[] nft_parts = new TextureRect[6];

	///<summary> The list the nft materials are stored in. </summary>
	private List<Texture> nfts = new List<Texture>();

	/// <summary> A collection of every asset used for generation. </summary>
	private List<Texture> nft_assets = new List<Texture>();

	///<summary> A default error nft inspired by the gmod error texture. </summary>
	private SpatialMaterial default_nft;

	/// <summary> A reference to the Autoload NFTResourceStorage class. </summary>
	private NFTResourceStorage storage;

	private Timer timer;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	private int counter = 0;


	public override void _Ready()
	{
		// Load a default error NFT
		default_nft = new SpatialMaterial();
		Texture err_img = GD.Load<Texture>("res://assets/Fake nfts/ErrorNFT.png");
		default_nft.AlbedoTexture = err_img;
		timer = GetNode<Timer>("Timer");
		rng.Randomize();

		storage = GetParent().GetNode<NFTResourceStorage>("NftResourceStorage");

		// Create and instantiate vars
		view = GetNode<Viewport>("ViewportContainer/Viewport");
		int index = 0;

		//Get NFT part nodes
		foreach(Node n in view.GetChildren())
		{
			if(n.Name.Contains("NFT_"))
			{
				nft_parts[index] = (TextureRect) n;
				index++;
			}
		}
	}

	/// <summary>
	/// Generates a set amount of nfts as SpatialMaterials and stores it in <paramref name="nfts"/>.
	/// Use <paramref name="getNFTMaterial"/> to get the results.
	/// </summary>
	/// <param name="amount">Amount of nfts to make.</param>
	public async void generateNFTs(int amount)
	{
		DONE_LOADING = false;
		for (int i = 0; i < amount; i++)
		{            
			// Change the nft's parts
			nft_parts[0].Texture = storage.getRandomAssetOfType(NFTAssetType.Background);
			nft_parts[1].Texture = storage.getRandomAssetOfType(NFTAssetType.Pattern);
			nft_parts[2].Texture = storage.getRandomAssetOfType(NFTAssetType.Eyes);
			nft_parts[3].Texture = storage.getRandomAssetOfType(NFTAssetType.Mouth);
			nft_parts[4].Texture = storage.getRandomAssetOfType(NFTAssetType.Hat);
			nft_parts[5].Texture = storage.getRandomAssetOfType(NFTAssetType.Accessory);
			await ToSignal(timer, "timeout");

			// Add the nft to the list
			mintNFT();
		}
	}

	/// <summary>
	/// Returns the requested nft from <paramref name="nfts"/> or
	/// returns a default texture.
	/// </summary>
	/// <param name="index">List index to return.</param>
	/// <returns>
	/// A material caputuring an nft from the generator.
	/// </returns>
	public Material getNFTMaterial(int index)
	{
		try
		{
			SpatialMaterial mat = new SpatialMaterial();
			mat.AlbedoTexture = nfts[index];
			return mat;
		}
		catch (ArgumentOutOfRangeException e)
		{
			GD.Print(e.StackTrace); // Print out where tf this is coming from
			return default_nft;
		}
	}

	/// <summary> Adds the current viewport texture of the NFT Creator scene to the <paramref name="nfts"/> list. </summary>
	private async void mintNFT()
	{
		// Save finished NFT
		Image image = view.GetTexture().GetData();
		//image.SavePng("testnft_" + counter + ".png"); // Uncomment to save generated NFT as a png (haha irony)
		//counter++;
		ImageTexture tex = new ImageTexture();
		tex.CreateFromImage(image);

		nfts.Add(tex);
		DONE_LOADING = true;
	}

	/// <summary> Gets the total amount of nfts created </summary>
	/// <returns> The amount of nfts made. </returns>
	public int getNFTCount()
	{
		return nfts.Count;
	}

	/// <summary> Dumps the contents of <paramref name="nfts"/> list. </summary>
	public void dumpNFTs()
	{
		nfts.Clear();
	}
}
