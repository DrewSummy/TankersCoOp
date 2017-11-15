/*

improve moving shooting ai: make aggressive ai tank have two more states where it circles when too close
delegate certain traits to ai tanks
idea: when player in sight stop moving or rotating and wait an amount of time then shoot
    improve by still rotating
    improve by not charging
idea: items to buy
    extra bullet
    map reveal
    point doubler for level
    extra life to respawn in start room
    idea: to increase effeciency dont run update on roomManagers, instead let enemy tanks have an instance of the room and decrement number of enemies with a function that once reaches 0 ends room
    
save scores

create inbetween level idles

save/continue/quit

include tutorial

create tutorial
learn cut scenes

improve aesthetics
better sounds (vocal maybe)
better art
make levels appear in creative ways
create explosion from last killed tank and push bullets players away
give player tanks have slightly different look than ai, maybe black outline
flashing red UI image over entire canvas when player dies/loses


idea:
    minigame: pushbattle where bullets don't kill they just push (damage multiplier dictates how far you get pushed)
    minigame: players get hit and a hole is placed where the player got hit so that the player would fall through
    minigame: race where bullets stun competitors
    bullet types: left trigger shoots slow, right- fast, both bidirectional bullets?
    teleport tank
    add background to tank game: two planes of dark brown, tilted cubes that move in waves
    if you kill the last tank after you died, flip a coin to see if you continue
    
bugs:
    projectile corner: projectiles ricochet unpredictably against corners of obstacles even when the corner is right against another obstacle


    give credit to https://github.com/daemon3000/InputManager/wiki/Getting-Started
*/
