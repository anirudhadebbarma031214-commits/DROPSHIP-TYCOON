# Naya Nagar: Indian Open-World Roleplay — GDD

## Vision
A compact, bright low-poly Indian city for mobile exploration, roleplay, jobs, shops, transport and seasonal festivals. This original Unity prototype uses procedural placeholder geometry until proper art assets are added.

## Core loop
Explore → take a delivery/job → earn rupees → buy snacks → discover landmarks → complete daily quests and seasonal activities.

## Systems
- Economy: start at ₹10,000. Jobs reward ₹450; snacks cost ₹120. Production code should route transactions through one EconomyService and versioned save data.
- Stamina: 0–100. Sprint drains 23 points/second; walking restores 10/second and resting restores 18/second. Food restores stamina.
- Daily quests: deliver 3 orders, visit a Mandir, and buy an item. Prototype resets by local calendar day; multiplayer should use server-authoritative UTC resets.
- Transport: walk/sprint, scooter/car placeholders, and bus-stop fast travel. Vehicle physics and steering need a later pass.
- Shops: ten original food outlets and a kirana snack purchase. Brand-inspired outlets should use original names/art unless licensed.
- Landmarks: three mall placeholders, stadium, Mandir, six fast-travel nodes, compact road grid and buildings.
- Diwali: night ambience toggle and diya interactions. Planned additions: rangoli, fairy lights, mithai delivery, safe-area fireworks and festive cosmetics.
- UI: cash/stamina upper-left, quest tracker upper-right, movement/sprint controls below, interaction/job/travel/shop/festival actions on the right.

## Roblox Lua architecture outline (for a future Roblox port)
- ServerScriptService/EconomyService: validate item prices and rewards on the server.
- ServerScriptService/QuestService: track daily progress by UserId and UTC date.
- StarterPlayerScripts/StaminaController: update the bar; server validates sprint rate and stamina.
- ReplicatedStorage/Remotes: request shop/job actions; server validates distance, cooldown, inventory and balance.
- Pseudocode: Buy(item) checks balance then deducts price and grants item; Sprint(dt) subtracts drainRate*dt; DailyReset compares savedDate to UTC date; DeliverOrder validates the active order and pays once.

## Roadmap
1. Prototype: generated city, avatar, touch HUD, jobs, snacks, fast travel, landmarks and Diwali toggle.
2. Playability: replace primitive buildings/avatars, add minimap, vehicle physics and interaction checks.
3. Content: detailed mall interiors, food interiors, cosmetics, stadium mini-games, festival quests and fireworks.
4. Polish: audio, accessibility, low-end Android profiling, persistence and localization.
5. Multiplayer: server-authoritative economy, anti-cheat, moderation and network testing.
