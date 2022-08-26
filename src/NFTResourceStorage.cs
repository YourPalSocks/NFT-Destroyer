using Godot;
using System;

public enum NFTAssetType {Accessory, Background, Eyes, Mouth, Pattern, Hat};
public class NFTResourceStorage : Node
{
    /*
    The game had a bug with the last system where the HTML export couldn't load the images on runtime.
    This class stores 
    */

    // Hard-coded image references
    [Export]
    private Texture[] Accessories;
    [Export]
    private Texture[] Backgrounds;
    [Export]
    private Texture[] Eyes;
    [Export]
    private Texture[] Mouths;
    [Export]
    private Texture[] Patterns;
    [Export]
    private Texture[] Hats;

    // Other vars
    RandomNumberGenerator rand;

    public override void _Ready()
    {
        rand = new RandomNumberGenerator();
        rand.Randomize();
    }

    public Texture getRandomAssetOfType(NFTAssetType type)
    {
        Texture toRet;
        switch(type)
        {
            case NFTAssetType.Accessory:
                toRet = Accessories[rand.RandiRange(0, Accessories.Length - 1)];
                return toRet;

            case NFTAssetType.Background:
                toRet = Backgrounds[rand.RandiRange(0, Backgrounds.Length - 1)];
                return toRet;

            case NFTAssetType.Eyes:
                toRet = Eyes[rand.RandiRange(0, Eyes.Length - 1)];
                return toRet;

            case NFTAssetType.Mouth:
                toRet = Mouths[rand.RandiRange(0, Mouths.Length - 1)];
                return toRet;

            case NFTAssetType.Pattern:
                toRet = Patterns[rand.RandiRange(0, Patterns.Length - 1)];
                return toRet;

            case NFTAssetType.Hat:
                toRet = Hats[rand.RandiRange(0, Hats.Length - 1)];
                return toRet;
        }
        return null;
    }
}
