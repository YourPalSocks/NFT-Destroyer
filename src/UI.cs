using Godot;
using System;

public class UI : Control
{
    private Label nfts_label;
    private Label coins_label;
    private Control pause_menu;
    private Control game_gui;
    private StoreManager shop_menu;
    private Label exclamation;

    private AnimationPlayer anim;

    public static bool pause_active = false;
    private bool in_shop = false;

    public override void _Ready()
    {
        nfts_label = GetNode<Label>("%NFTsDestroyed");
        coins_label = GetNode<Label>("%ElixerCollected");
        game_gui = GetChild<Control>(0);
        pause_menu = GetChild<Control>(1);
        shop_menu = GetChild<StoreManager>(2);
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        exclamation = GetNode<Label>("%Exclamation");
        // Connect pause menu signals
        pause_menu.Visible = false;
        shop_menu.Visible = false;
        shop_menu.GetNode("%Return Button").Connect("pressed", this, "_on_Return_Button_pressed");
        shop_menu.Connect("store_sale", this, "_on_Store_Sale");
    }

    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("player_pause") && !in_shop)
        {
            pause_active = !pause_active;

            if(pause_active) // Pause is on
            {
                game_gui.Visible = false;
                pause_menu.Visible = true;
                GetTree().Paused = true;
            }
            else // Pause is off
            {
                game_gui.Visible = true;
                pause_menu.Visible = false;
                GetTree().Paused = false;
            }
        }

    }

    public void updateUI()
    {
        nfts_label.Text = "NFTs Destroyed: " + GameManager.nfts_destroyed.ToString();
        coins_label.Text = "Elixerium Collected: " + GameManager.coins_collected.ToString();
    }

    public void _on_Shop_Button_pressed()
    {
        shop_menu.Visible = true;
        game_gui.Visible = false;
        GetTree().Paused = true;
        in_shop = true;
        GameManager.can_throw_matches = true;
        // Update store UI
        shop_menu.updateUI();

    }

    public void _on_Return_Button_pressed()
    {
        shop_menu.Visible = false;
        game_gui.Visible = true;
        updateUI();
        GetTree().Paused = false;
        in_shop = false;
    }

    public void _on_Shop_Button_mouse_entered()
    {
        GameManager.can_throw_matches = false;
    }

    public void _on_Shop_Button_mouse_exited()
    {
        GameManager.can_throw_matches = true;
    }

    public void _on_Store_Sale()
    {
        anim.Play("Store Sale");
    }
}
