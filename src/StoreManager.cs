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
    private int gpu_coin_cost = 5;
    private int match_down_cost = 15;
    private int pump_dump_amt = 20;
    private int market_amt = 45;

    // Price upgrade
    private int min_price_change = -10;
    private int max_price_change = 10;

    private int market_upgrades = 0;

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

    public override void _Process(float delta)
    {
        if(Visible && !timer.Paused)
        {
            timer.Paused = true;
        }
        else if (!Visible && timer.Paused)
        {
            timer.Paused = false;
        }
    }

    public void updateUI()
    {
        nft_counter.Text = "NFTs: " + GameManager.nfts_destroyed.ToString();
        coin_counter.Text = "Elixerium: " + GameManager.coins_collected.ToString();
        GetNode("%Miner Upgrade").GetChild<Label>(0).Text = "Miner Upgrade: $" + gpu_coin_cost.ToString();
        GetNode("%Match Upgrade").GetChild<Label>(0).Text = "Match Cooldown Reduction: $" + match_down_cost.ToString();
        GetNode("%PD Upgrade").GetChild<Label>(0).Text = "Pump/Dump Elixerium: $" + pump_dump_amt.ToString();
        GetNode("%Market Upgrade").GetChild<Label>(0).Text = "Market Manipulation: $" + market_amt.ToString();
    }

    private void _on_Miner_Upgrade_pressed()
    {
        if(manager.getActiveGpus() < GameManager.MAX_GPUS && GameManager.coins_collected >= gpu_coin_cost)
        {
            confirm_sound.Play();
            manager.activateGpu();
            // Update UI and price
            GameManager.coins_collected -= gpu_coin_cost;
            gpu_coin_cost += 8;
            GetNode("%Miner Upgrade").GetChild<Label>(1).Text = manager.getActiveGpus().ToString() + " ";
            GetNode("%Miner Upgrade").GetChild<Label>(0).SelfModulate = Colors.White;
            updateUI();
        }
        else
        {
            // Can't get the upgrade, play deny sound
            deny_sound.Play();
        }
    }

    private void _on_Match_Upgrade_pressed()
    {
        if(manager.getMatchCooldownAmt() < GameManager.MAX_MATCHES && GameManager.coins_collected >= match_down_cost)
        {
            confirm_sound.Play();
            manager.reduceMatchAmt();
            // Update UI and price
            GameManager.coins_collected -= match_down_cost;
            match_down_cost += 13;
            GetNode("%Match Upgrade").GetChild<Label>(1).Text = manager.getMatchCooldownAmt().ToString() + " ";
            GetNode("%Match Upgrade").GetChild<Label>(0).SelfModulate = Colors.White;
            updateUI();
        }
        else
        {
            deny_sound.Play();
        }
    }

    private void _on_PD_Upgrade_pressed()
    {
        if(manager.getPumpAmt() < GameManager.MAX_PUMP && GameManager.coins_collected >= pump_dump_amt)
        {
            confirm_sound.Play();
            manager.addCoinVariety();
            // Update UI and price
            GameManager.coins_collected -= pump_dump_amt;
            pump_dump_amt += 20;
            GetNode("%PD Upgrade").GetChild<Label>(1).Text = manager.getPumpAmt().ToString() + " ";
            GetNode("%PD Upgrade").GetChild<Label>(0).SelfModulate = Colors.White;
            updateUI();
        }
        else
        {
            deny_sound.Play();
        }
    }

    private void _on_Market_Upgrade_pressed()
    {
        if(market_upgrades < GameManager.MAX_MARKET && GameManager.coins_collected >= market_amt)
        {
            confirm_sound.Play();
            min_price_change -= 1;
            max_price_change += 1;
            // Update UI and price
            GameManager.coins_collected -= market_amt;
            market_amt += 25;
            GetNode("%Market Upgrade").GetChild<Label>(1).Text = market_upgrades.ToString() + " ";
            GetNode("%Market Upgrade").GetChild<Label>(0).SelfModulate = Colors.White;
            updateUI();
        }
        else
        {
            deny_sound.Play();
        }
    }

    // Market Event: change prices
    private void _on_Timer_timeout()
    {
        // Shuffle GPU price
        int temp = 0;
        temp = changePrice(gpu_coin_cost);
        compareAndChangeColor(GetNode("%Miner Upgrade").GetChild<Label>(0), gpu_coin_cost, temp);
        gpu_coin_cost = temp;

        // Shuffle Match cooldown price
        temp = changePrice(match_down_cost);
        compareAndChangeColor(GetNode("%Match Upgrade").GetChild<Label>(0), match_down_cost, temp);
        match_down_cost = temp;

        // Shuffle pump price
        temp = changePrice(pump_dump_amt);
        compareAndChangeColor(GetNode("%PD Upgrade").GetChild<Label>(0), pump_dump_amt, temp);
        pump_dump_amt = temp;

        // Shuffle market price
        temp = changePrice(market_amt);
        compareAndChangeColor(GetNode("%Market Upgrade").GetChild<Label>(0), market_amt, temp);
        market_amt = temp;

        // Reset the timer
        timer.WaitTime = 60 + rng.RandiRange(-15, 60);
        // Emit signal to send notif to player
        EmitSignal("store_sale");
        // Update store UI
        updateUI();
    }

    private void compareAndChangeColor(Label label, int old_val, int new_val)
    {
        if(new_val > old_val) // Worse price, highlight red
        {
            label.SelfModulate = Colors.PaleVioletRed;
        }
        else if (new_val < old_val) // Better price, highlight green
        {
            label.SelfModulate = Colors.LightGreen;
        }
        else
        {
            label.SelfModulate = Colors.White;
        }
    }

    private int changePrice(int start)
    {
        // Change price by adding random value
        start += rng.RandiRange(min_price_change, max_price_change);
        if(start <= 0) // No negative or zero prices
        {
            start = 1;
        }
        return start;
    }
}
