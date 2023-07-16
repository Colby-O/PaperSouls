using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Interfaces
{
    internal interface IUseables
    {
        /// <summary>
        /// Use an object
        /// </summary>
        public void Use(PlayerManger player);

        /// <summary>
        /// Use an object and output wheather was sucessfully used or not
        /// </summary>
        public void Use(PlayerManger player, out bool sucessful);
    }
}
