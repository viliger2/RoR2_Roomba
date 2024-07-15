using JetBrains.Annotations;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2_Roomba
{
    public class RoombaInteractableManager : NetworkBehaviour, IInteractable
    {
        public CharacterBody body;

        public GameObject smokeBombPrefab;

        public string GetContextString([NotNull] Interactor activator)
        {
            return RoR2.Language.GetString("ROOMBA_INTERACTABLE_ROOMBA_CONTEXT");
        }

        public Interactability GetInteractability([NotNull] Interactor activator)
        {
            int count = activator?.GetComponent<CharacterBody>()?.inventory?.GetItemCount(ContentProvider.Items.PileOfDirt) ?? 0;
            if(count > 0)
            {
                return Interactability.Available;
            }
            return Interactability.Disabled;
        }

        public void OnInteractionBegin([NotNull] Interactor activator)
        {
            if(!NetworkServer.active)
            {
                return;
            }

            if (body && body.healthComponent)
            {
                activator?.GetComponent<CharacterBody>()?.inventory?.RemoveItem(ContentProvider.Items.PileOfDirt);
                Invoke("SpawnSmokeEffect", 1f);
                Invoke("DestroyRoomba", 1.5f);
            }
        }

        private void SpawnSmokeEffect()
        {
            EffectManager.SimpleMuzzleFlash(smokeBombPrefab, body.gameObject, "SmokeBomb", true);
            EntitySoundManager.EmitSoundServer((AkEventIdArg)"Roomba_HL_Explosion_Play", body.gameObject);
        }

        private void DestroyRoomba()
        {
            body.healthComponent.Suicide();
        }

        public bool ShouldIgnoreSpherecastForInteractibility([NotNull] Interactor activator)
        {
            return false;
        }

        public bool ShouldShowOnScanner()
        {
            return true;
        }
    }
}
