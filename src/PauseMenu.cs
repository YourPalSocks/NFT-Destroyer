using Godot;
using System;

public class PauseMenu : Control
{
    // Settings Events
    private void _on_Music_Slider_value_changed(float value)
    {
        int music_index = AudioServer.GetBusIndex("Music");
        AudioServer.SetBusVolumeDb(music_index, value);
    }

    private void _on_Sound_Slider_value_changed(float value)
    {
        int music_index = AudioServer.GetBusIndex("SFX");
        AudioServer.SetBusVolumeDb(music_index, value);
    }
}
