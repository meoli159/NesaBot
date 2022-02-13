using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NesaBot.Core.Managers;
using System.Threading.Tasks;

namespace NesaBot.Core.Commands
{
    [Name("Music")]
    public class MusicCommands : ModuleBase<SocketCommandContext>
    {
        [Command("join")]
        [Summary("Makes Nesa join VC.")]
        public async Task JoinCommand()
            => await Context.Channel.SendMessageAsync(await AudioManager.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));

        [Command("play")]
        [Summary("Play video from ytb.")]
        public async Task PlayCommand([Remainder] string search)
            => await Context.Channel.SendMessageAsync(await AudioManager.PlayAsync(Context.User as SocketGuildUser, Context.Guild, search));


        [Command("leave")]
        [Summary("leave the vc.")]
        public async Task LeaveCommand()
            => await Context.Channel.SendMessageAsync(await AudioManager.LeaveAsync(Context.Guild));

        [Command("pause")]
        [Alias("resume")]
        [Summary("pause/resume music")]
        public async Task PauseCommand()
            => await Context.Channel.SendMessageAsync(await AudioManager.TogglePauseAsync(Context.Guild));

        [Command("stop")]
        [Summary("stop all music")]
        public async Task StopCommand()
            => await Context.Channel.SendMessageAsync(await AudioManager.StopAsync(Context.Guild));

        [Command("skip")]
        [Summary("skip the music")]
        public async Task SkipCommand()
            => await Context.Channel.SendMessageAsync(await AudioManager.SkipAsync(Context.Guild));
    }
}
