using System;
using System.Collections;
using System.Collections.Generic;
using KindMen.Uxios.Api;
using NUnit.Framework;
using RSG;
using UnityEngine.TestTools;

namespace KindMen.Uxios.Tests
{
    /// <summary>
    /// The goal of this test is to demonstrate how easy it is to fetch data from a real life example, in this case
    /// the PokeApi.
    ///
    /// The other tests verify behaviour by being technical about it, but this is our playground to really show how
    /// you -the user- can use this in your own application.
    /// </summary>
    public class PokeApi
    {
        public class Ability
        {
            public string name;
            public Uri url;
        }

        public class PokemonAbility
        {
            public bool is_hidden;
            public int slot;
            public Ability ability;
        }
        
        /// <summary>
        /// See https://pokeapi.co/docs/v2#pokemon for the reference behind this class
        /// </summary>
        public class Pokemon
        {
            public int id;
            public string name;
            public int base_experience;
            public int height;
            public bool is_default;
            public int order;
            public int weight;
            public List<PokemonAbility> abilities;
        }

        public class PokemonApi
        {
            public IPromise<Pokemon> GetPokemonAsync(string name)
            {
                return Resource<Pokemon>.At("https://pokeapi.co/api/v2/pokemon/" + name).Value;
            }
        }

        private PokemonApi pokemonApi;

        [SetUp]
        public void SetUp()
        {
            this.pokemonApi = new PokemonApi();
        }

        [UnityTest]
        public IEnumerator GetDitto()
        {
            var promise = pokemonApi.GetPokemonAsync("ditto")
                .Then(pokemon =>
                {
                    Assert.That(pokemon.name, Is.EqualTo("ditto"));
                });

            // because this is a test .. we need to make this blocking. Boo!
            yield return Uxios.WaitForRequest(promise);
            
            // dit is na de promise
        }
    }
}