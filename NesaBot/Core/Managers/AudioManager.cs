using Victoria;
using Microsoft.Extensions.DependencyInjection;
using Victoria.EventArgs;
using Discord;
using Discord.WebSocket;
using Victoria.Enums;

namespace NesaBot.Core.Managers
{
    public static class AudioManager
    {
        private static readonly LavaNode _lavaNode = ServiceManager.Provider.GetRequiredService<LavaNode>();
        
        public static async Task<string> JoinAsync(IGuild guild, IVoiceState voiceState, ITextChannel channel)
        {
            if (_lavaNode.HasPlayer(guild)) return "I'm already in a voice channel.";

            if (voiceState.VoiceChannel is null) return "You must be connected to Voice channel master!!";

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, channel);
                return $"Joined {voiceState.VoiceChannel.Name}";
            }
            catch (Exception ex)
            {
                return $"ERROR\n{ex.Message}";
            }
        }

        public static async Task<string> PlayAsync(SocketGuildUser user, IGuild guild, string query)
        {
            if (user.VoiceChannel is null) return "Master! You must join a voice channel";

            if (!_lavaNode.HasPlayer(guild)) return "I'm not connect to the voice channeel :(";

            try
            {
                var player = _lavaNode.GetPlayer(guild);

                

                var search = Uri.IsWellFormedUriString(query, UriKind.Absolute)
                    ? await _lavaNode.SearchAsync(query)
                    : await _lavaNode.SearchYouTubeAsync(query);

                if (search.LoadStatus == LoadStatus.NoMatches) return $"I couldn't find anything for {query}";

                var track = search.Tracks.FirstOrDefault();

                if (player.Track != null && player.PlayerState is PlayerState.Playing ||
                    player.PlayerState is PlayerState.Paused)
                {
                    player.Queue.Enqueue(track);
                    Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\tTrack was added to queue");
                    return $"{track.Title} has been added to queue";
                }
                else
                {
                    await player.PlayAsync(track);
                    Console.WriteLine($"Now playing: {track.Title}");
                    return $"Now playing:{track.Title}";
                }
                
            }
            catch(Exception ex)
            {
                return $"ERROR:\t{ex.Message}";
            }
        }

        public static async Task<string> LeaveAsync(IGuild guild)
        {
            try
            {
                var player = _lavaNode?.GetPlayer(guild);
                if(player.PlayerState is PlayerState.Playing) await player.StopAsync();
                await _lavaNode.LeaveAsync(player.VoiceChannel);

                Console.WriteLine($"[{DateTime.Now}]\t(AUDIO)\tI has left a voice channel");
                return "I have left a voice channel! ";
            }
            catch (InvalidOperationException ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static async Task<string> TogglePauseAsync(IGuild guild)
        {
            try
            {
                var player = _lavaNode?.GetPlayer(guild);
                if (!(player.PlayerState is PlayerState.Playing) && player.Track == null)
                        return "There is nothing playing now master !!";
                if (!(player.PlayerState is PlayerState.Playing))
                {
                    await player.ResumeAsync();
                    return $"**Resumed** { player.Track.Title}";
                }

                await player.PauseAsync();
                return $"**Paused** { player.Track.Title}, kaboom!!!! ";
                
            }
            catch (InvalidOperationException ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static async Task<string> StopAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.Track != null && player.PlayerState is PlayerState.Playing ||
                    player.PlayerState is PlayerState.Paused)
            {
                await player.StopAsync();
                return "Player stopped.";
            }
            else
            {
                return "Nothing is playing";
            }
        }

        public static async Task<string> SkipAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.Queue.Count == 0 )
            {
                
                return "Nothing to skip";
            }
            if (player.Track != null && player.PlayerState is PlayerState.Playing ||
                    player.PlayerState is PlayerState.Paused)
            {
                await player.SkipAsync();
                return $"Song skipped,Now playing: {player.Track.Title}.";
            }
                return "Nothing is currently playing";
            
        }

        public static async Task TrackEnded(TrackEndedEventArgs args, LavaTrack track)
        {
            if (!args.Reason.ShouldPlayNext()) return;
            
            if(!args.Player.Queue.TryDequeue(out var queueable)) return;

            if (!(queueable is LavaTrack nexttrack))
            {
                await args.Player.TextChannel.SendMessageAsync("Next item in the queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(nexttrack);
            await args.Player.TextChannel.SendMessageAsync($"Now Playing *{track.Title} - {track.Author}*");


        }

    }
}
