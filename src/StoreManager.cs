using Godot;
using System;

public class StoreManager : Control
{
    // Componenets
    private Label nft_counter;
    private Label coin_counter;
    private GameManager manager;
    private Timer timer;
    private AudioStreamPlayer confirm_sound;
    private AudioStreamPlayer deny_sound;

    private RandomNumberGenerator rng = new RandomNumberGenerator();

    [Signal]
    delegate void store_sale();

    // Prices
    private int gpu_coin_cost = 1;

    public override void _Ready()
    {
        nft_counter = GetNode<Label>("%NFT Counter");
        coin_counter = GetNode<Label>("%Elixer Counter");
        manager = GetParent().GetParent<GameManager>();
        timer = GetNode<Timer>("Timer");
        confirm_sound = GetNode<AudioStreamPlayer>("ConfirmSound");
        deny_sound = GetNode<AudioStreamPlayer>("DenySound");
        rng.Randomize();
    }


    public void updateUI()
    {
        nft_counter.Text = "NFTs: " + GameManager.nfts_destroyed.ToString();
        coin_counter.Text = "Elixerium: " + GameManager.coins_collected.ToString();
        GetNode("%Miner Upgrade").GetChild<Label>(0).Text = "Miner Upgrade: $" + gpu_coin_cost.ToString();
    }

    private void _on_Miner_Upgrade_pressed()
    {
        if(manager.getActiveGpus() < GameManager.MAX_GPUS && GameManager.coins_collected >= gpu_coin_cost)
        {
            confirm_sound.Play();
            manager.activateGpu();
            // Update UI and price
            GameManager.coins_collected -= gpu_coin_cost;
            gpu_coin_cost += 5;
            GetNode("%Miner Upgrade").GetChild<Label>(1).Text = manager.getActiveGpus().ToString() + " ";
            updateUI();
        }
        else
        {
            // Can't get the upgrade, play deny sound
            deny_sound.Play();
        }
    }

    private void _on_Timer_timeout()
    {
        // Shuffle some prices
        gpu_coin_cost += rng.RandiRange(-10, 10);
        // No negative values
        if(gpu_coin_cost < 0)
        {
            gpu_coin_cost = 1;
        }
        timer.WaitTime = 60 + rng.RandiRange(-15, 60);
        // Emit signal to send notif to player
        EmitSignal("store_sale");
        // Update store UI
        updateUI();
    }
}
