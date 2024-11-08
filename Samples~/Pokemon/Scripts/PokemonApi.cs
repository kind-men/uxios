using KindMen.Uxios.Api;
using KindMen.Uxios.PokemonObjects;
using RSG;

namespace KindMen.Uxios
{
    public class PokemonApi
    {
        public IPromise<Pokemon> GetPokemon(string name)
        {
            var pokemonResource = Resource<Pokemon>.At("https://pokeapi.co/api/v2/pokemon/" + name);

            return pokemonResource.Value;
        }
    }
}