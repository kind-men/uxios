using System;
using KindMen.Uxios.Api;
using KindMen.Uxios.PokemonObjects;
using RSG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KindMen.Uxios.UI
{
    public class PokemonUI : MonoBehaviour
    {
        private PokemonApi pokemonApi = new();

        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text nameField;
        [SerializeField] private Image avatarField;

        [SerializeField] private PromiseLoaderPanel loaderPanel;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private TMP_Text errorTextField;

        public void OnButtonClick()
        {
            IPromise<Pokemon> getPokemon = pokemonApi.GetPokemon(inputField.text);
            
            // This function will 'decorate' the promise and show a loader during execution, and hide the loader
            // upon completion or error
            loaderPanel.ShowWhile(getPokemon);
            
            // When requesting a pokemon succeeds, hide the error panel and update the UI ..
            getPokemon.Then(_ => HideErrorPanel());
            getPokemon.Then(UpdateUI);
            
            // BUT: when the request fails, we show an error
            getPokemon.Catch(ShowError);
        }
        
        private void UpdateUI(Pokemon pokemon)
        {
            infoPanel.SetActive(true);
            nameField.text = pokemon.name;

            LoadAvatar(pokemon);
        }

        private void LoadAvatar(Pokemon pokemon)
        {
            var sprite = pokemon.sprites.front_default;
            
            // Using a Resource object, you can also directly query a URL; and the generic type Sprite will
            // automagically retrieve the url as an image, load it into a texture and convert that texture into
            // a sprite - easy is it not?!
            Promise<Sprite> getAvatar = Resource<Sprite>.At(sprite).Value;
            getAvatar.Then(SetAvatarImage);
            
            // ... you can also use lambda's if you want
            getAvatar.Catch(e => Debug.Log("Could not load avatar: " + e.Message));
        }

        private void SetAvatarImage(Sprite sprite)
        {
            avatarField.sprite = sprite;
        }

        private void ShowError(Exception error)
        {
            HideInfoPanel();
            errorPanel.SetActive(true);
            errorTextField.text = ((Error)error).Message;
        }

        private void HideInfoPanel()
        {
            infoPanel.SetActive(false);
        }

        private void HideErrorPanel()
        {
            errorPanel.SetActive(false);
        }
    }
}