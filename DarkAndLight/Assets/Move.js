#pragma strict
var distance = 0.1;
var time =  0.1;
function Start () {

}
function fire ()
{
	print("I fired!");
}
function Update () {


	if(Input.GetKey("w"))
	{
	transform.position.x += distance * time;
	}
	if(Input.GetKeyDown("s"))
	{
	fire();
	}
}
