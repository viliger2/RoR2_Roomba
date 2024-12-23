﻿using JetBrains.Annotations;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2_Roomba
{
    public class RoombaInteractableManager : NetworkBehaviour, IInteractable
    {
        [SyncVar]
        public bool available;

        public CharacterBody body;

        public GameObject smokeBombPrefab;

        private CharacterBody interactor;

        public void Awake()
        {
            available = true;
        }

        public string GetContextString([NotNull] Interactor activator)
        {
            return RoR2.Language.GetString("ROOMBA_INTERACTABLE_ROOMBA_CONTEXT");
        }

        public Interactability GetInteractability([NotNull] Interactor activator)
        {
            if (!available)
            {
                return Interactability.Disabled;
            }

            if (!activator || !activator.TryGetComponent<CharacterBody>(out var characterBody))
            {
                return Interactability.Disabled;
            }

            if(!characterBody.inventory)
            {
                return Interactability.Disabled;
            }

            int count = characterBody.inventory.GetItemCount(ContentProvider.Items.PileOfDirt);
            if (count > 0)
            {
                return Interactability.Available;
            }

            return Interactability.Disabled;
        }

        public void OnInteractionBegin([NotNull] Interactor activator)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (body && body.healthComponent && available)
            {
                available = false;
                body.AddBuff(RoR2Content.Buffs.Slow80);
                interactor = activator?.GetComponent<CharacterBody>();
                interactor?.inventory?.RemoveItem(ContentProvider.Items.PileOfDirt);
                EntitySoundManager.EmitSoundServer((AkEventIdArg)"Roomba_Breaking_Play", body.gameObject);
                Invoke("SpawnSmokeEffect", 4f);
                Invoke("DestroyRoomba", 5f);
            }
        }

        private void SpawnSmokeEffect()
        {
            EffectManager.SimpleMuzzleFlash(smokeBombPrefab, body.gameObject, "SmokeBomb", true);
            EntitySoundManager.EmitSoundServer((AkEventIdArg)"Roomba_HL_Explosion_Play", body.gameObject);
            if (interactor)
            {
                Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
                {
                    subjectAsCharacterBody = interactor,
                    baseToken = "ROOMBA_INTERACTABLE_CHAT_MESSAGE_BROKEN"
                });
            }
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

        public bool ShouldProximityHighlight()
        {
            return true;
        }
    }
}
