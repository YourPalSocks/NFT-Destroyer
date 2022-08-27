using Godot;
using System;

public class GameManager : Node
{
    // NFT Assets
    [Export]
    private PackedScene nft;
    private NFT curNft;
    private NFTGenerator block_chain;

    // Coin collectibles
    [Export]
    private PackedScene coin;
    private Vector3 COIN_START = new Vector3(-0.48f, 2f, -1.8f);
    RandomNumberGenerator rng;
    private const float COIN_DROP_TIME = 40f;
    private float cur_coin_drop;

    // Shop things
    private Spatial gpu_storage;
    private int gpus_active = 0;
    private int match_down = 0;
    private int pump_amt = 0;

    // Shop contrains
    public const int MAX_GPUS = 10;
    public const int MAX_MATCHES = 5;
    public const int MAX_PUMP = 5;

    private UI ui;
    private PlayerScript player;

    bool has_run = false;
    private Vector3 nft_pos = new Vector3(0, 0, -2.258f);

    public static bool can_throw_matches = true;
    public static int nfts_destroyed = 0;
    public static int coins_collected = 0;

    public override void _Ready()
    {
        // Get object references
        block_chain = GetTree().Root.GetNode<NFTGenerator>("NftCreator");
        ui = GetNode<UI>("UI");
        gpu_storage = GetNode<Spatial>("GPU PowerUps");
        player = GetNode<PlayerScript>("Camera");

        // NFT Gen
        rng = new RandomNumberGenerator();
        rng.Randomize();

        cur_coin_drop = COIN_DROP_TIME;
    }

    private void _on_Timer_timeout()
    {
        spawnNewNFT(); // Spawn first NFT
    }

    public override void _Process(float delta)
    {
        if(curNft == null)
        {
            return;
        }

        // NFT Generation
        if(NFTGenerator.DONE_LOADING && !has_run)
        {
            curNft.replaceTexture(block_chain.getNFTMaterial(0));
            curNft.Visible = true;
            has_run = true;
            block_chain.dumpNFTs();
        }

        // Coin drops
        if(cur_coin_drop <= 0)
        {
            spawnCoin();
            // Reset timer, with a little randomization
            cur_coin_drop = COIN_DROP_TIME + rng.RandiRange(-6, 6);
        }
        else
        {
            cur_coin_drop -= 0.1f;
        }
    }

    private void _on_NFT_tree_exited()
    {
        spawnNewNFT();
        nfts_destroyed++;

        if(nfts_destroyed % 5 == 0) // Free coin for every 5 NFTs burned
        {
            spawnCoin();
        }

        // Update UI
        ui.updateUI();
    }

    public void addCoin(int amt)
    {
        coins_collected += amt;

        // Update UI
        ui.updateUI();
    }

    private void spawnCoin()
    {
        float x_coor = rng.RandfRange(-1.3f, 1.3f);
        float z_rot = rng.RandiRange(-77, 77);

        Collectable new_coin = (Collectable) coin.Instance();
        AddChild(new_coin);
        new_coin.Translation = new Vector3(x_coor, COIN_START.y, COIN_START.z);
        new_coin.GetChild<Spatial>(0).RotationDegrees = new Vector3(0, z_rot, 0);

        // Set value based on upgrade
        if(pump_amt > 0)
        {
            new_coin.setCoinAmt(rng.RandiRange(1, pump_amt + 1));
        }
    }

    private void spawnNewNFT()
    {
        block_chain.generateNFTs(1);
        // TODO: Slide-in effects
        NFT new_nft = (NFT) nft.Instance();
        new_nft.Visible = false;
        CallDeferred("add_child", new_nft);
        new_nft.Connect("tree_exiting", this, "_on_NFT_tree_exited");
        new_nft.Translation = nft_pos;

        curNft = new_nft;
        has_run = false;
    }

    public int getActiveGpus()
    {
        return gpus_active;
    }

    public int getMatchCooldownAmt()
    {
        return match_down;
    }

    public int getPumpAmt()
    {
        return pump_amt;
    }

    public void activateGpu()
    {
        gpus_active++;
        GPUScript gpu = gpu_storage.GetChild<GPUScript>(gpus_active - 1);
        gpu.Visible = true;
        gpu.Connect("gpu_timeout", this, "_add_Coin_From_GPU");
    }

    public void reduceMatchAmt()
    {
        match_down++;
        player.reduceMatchTime(1.2f);
    }

    public void addCoinVariety()
    {
        pump_amt++;
    }

    private void _add_Coin_From_GPU()
    {
        coins_collected++;
        ui.updateUI();
    }
}
