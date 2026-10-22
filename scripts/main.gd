extends Node3D

const STARTING_CASH := 500
var cash := STARTING_CASH
var income_per_second := 5
var owned_buildings := 0
var player: CharacterBody3D
var cash_label: Label
var status_label: Label
var hold_forward := false
var hold_back := false
var hold_left := false
var hold_right := false
var tick := 0.0

func _ready() -> void:
	_build_world()
	_build_player()
	_build_ui()
	_update_hud()

func _process(delta: float) -> void:
	tick += delta
	if tick >= 1.0:
		tick = 0.0
		cash += income_per_second
		_update_hud()

func _build_world() -> void:
	var world_env := WorldEnvironment.new()
	var environment := Environment.new()
	environment.background_mode = Environment.BG_SKY
	environment.sky = Sky.new()
	environment.sky.sky_material = ProceduralSkyMaterial.new()
	environment.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	environment.ambient_light_color = Color(0.72, 0.80, 0.92)
	environment.ambient_light_energy = 0.8
	world_env.environment = environment
	add_child(world_env)

	var sun := DirectionalLight3D.new()
	sun.rotation_degrees = Vector3(-52, -30, 0)
	sun.light_energy = 1.2
	add_child(sun)

	_make_box("Ground", Vector3(0, -0.35, 0), Vector3(80, 0.5, 80), Color(0.24, 0.48, 0.29))
	_make_box("Main road", Vector3(0, -0.08, 0), Vector3(10, 0.08, 80), Color(0.13, 0.15, 0.18))
	for z in range(-36, 40, 8):
		_make_box("Road marking", Vector3(0, -0.025, z), Vector3(0.22, 0.03, 3.0), Color(0.96, 0.88, 0.57))

	# A small town skyline made from full 3D buildings.
	var building_colors := [Color(0.82,0.55,0.32), Color(0.72,0.78,0.82), Color(0.88,0.76,0.48), Color(0.53,0.68,0.73), Color(0.78,0.58,0.55)]
	for i in range(12):
		var side := -1.0 if i % 2 == 0 else 1.0
		var z := -32.0 + float(i) * 5.8
		var h := 3.5 + float((i * 7) % 5)
		var x := side * (9.0 + float(i % 3) * 3.0)
		_make_box("Town building %d" % i, Vector3(x, h / 2.0, z), Vector3(4.0, h, 4.2), building_colors[i % building_colors.size()])
		for floor_i in range(1, int(h / 1.5) + 1):
			_make_box("Window", Vector3(x - side * 2.02, float(floor_i) * 1.35, z), Vector3(0.05, 0.48, 0.7), Color(0.23, 0.72, 0.88))

	_make_box("Tycoon plot", Vector3(17, 0.02, 0), Vector3(12, 0.15, 12), Color(0.40, 0.42, 0.34))
	_make_box("Starter shop", Vector3(17, 1.4, -2), Vector3(5.5, 2.8, 4.0), Color(0.12, 0.46, 0.67))
	_make_box("Shop roof", Vector3(17, 2.95, -2), Vector3(6.0, 0.25, 4.5), Color(0.08, 0.18, 0.28))
	_make_box("Shop sign", Vector3(17, 2.0, 0.08), Vector3(2.8, 0.65, 0.12), Color(0.98, 0.73, 0.16))
	_make_box("Parking pad", Vector3(17, 0.05, 8), Vector3(7, 0.12, 5), Color(0.20, 0.22, 0.24))
	_make_box("Car body", Vector3(17, 0.75, 8), Vector3(2.4, 0.75, 4.0), Color(0.82, 0.12, 0.10))
	_make_box("Car cabin", Vector3(17, 1.25, 8), Vector3(1.7, 0.65, 1.8), Color(0.16, 0.25, 0.30))

func _make_box(label: String, pos: Vector3, size: Vector3, color: Color) -> MeshInstance3D:
	var box := MeshInstance3D.new()
	box.name = label
	var mesh := BoxMesh.new()
	mesh.size = size
	box.mesh = mesh
	var material := StandardMaterial3D.new()
	material.albedo_color = color
	material.roughness = 0.82
	box.material_override = material
	box.position = pos
	add_child(box)
	# Give the ground and buildings real collision so the player can walk around.
	var body := StaticBody3D.new()
	body.name = label + " Collision"
	body.position = pos
	var collision := CollisionShape3D.new()
	var shape := BoxShape3D.new()
	shape.size = size
	collision.shape = shape
	body.add_child(collision)
	add_child(body)
	return box

func _build_player() -> void:
	player = CharacterBody3D.new()
	player.name = "Player"
	player.position = Vector3(0, 1.2, 12)
	add_child(player)

	var capsule := MeshInstance3D.new()
	var capsule_mesh := CapsuleMesh.new()
	capsule_mesh.radius = 0.42
	capsule_mesh.height = 1.8
	capsule.mesh = capsule_mesh
	var player_mat := StandardMaterial3D.new()
	player_mat.albedo_color = Color(0.10, 0.72, 0.95)
	capsule.material_override = player_mat
	player.add_child(capsule)

	var collision := CollisionShape3D.new()
	var shape := CapsuleShape3D.new()
	shape.radius = 0.42
	shape.height = 1.8
	collision.shape = shape
	player.add_child(collision)

	var camera := Camera3D.new()
	camera.position = Vector3(0, 5.0, 8.5)
	camera.rotation_degrees = Vector3(-23, 0, 0)
	camera.current = true
	player.add_child(camera)

func _physics_process(_delta: float) -> void:
	if player == null:
		return
	var direction := Vector3.ZERO
	if hold_forward or Input.is_key_pressed(KEY_W) or Input.is_key_pressed(KEY_UP):
		direction.z -= 1.0
	if hold_back or Input.is_key_pressed(KEY_S) or Input.is_key_pressed(KEY_DOWN):
		direction.z += 1.0
	if hold_left or Input.is_key_pressed(KEY_A) or Input.is_key_pressed(KEY_LEFT):
		direction.x -= 1.0
	if hold_right or Input.is_key_pressed(KEY_D) or Input.is_key_pressed(KEY_RIGHT):
		direction.x += 1.0
	direction = direction.normalized()
	player.velocity.x = direction.x * 6.0
	player.velocity.z = direction.z * 6.0
	player.velocity.y -= 22.0 * get_physics_process_delta_time()
	if player.is_on_floor() and player.velocity.y < 0:
		player.velocity.y = -0.5
	player.move_and_slide()

func _build_ui() -> void:
	var canvas := CanvasLayer.new()
	add_child(canvas)
	var panel := PanelContainer.new()
	panel.position = Vector2(18, 16)
	panel.custom_minimum_size = Vector2(300, 108)
	canvas.add_child(panel)
	var column := VBoxContainer.new()
	panel.add_child(column)
	var title := Label.new()
	title.text = "DROPSHIP TYCOON  |  ANI STUDIO"
	title.add_theme_font_size_override("font_size", 18)
	column.add_child(title)
	cash_label = Label.new()
	cash_label.add_theme_font_size_override("font_size", 23)
	column.add_child(cash_label)
	status_label = Label.new()
	status_label.text = "Walk to your plot and grow your business!"
	column.add_child(status_label)

	_add_hold_button(canvas, "▲", Vector2(42, -150), "forward")
	_add_hold_button(canvas, "▼", Vector2(42, -82), "back")
	_add_hold_button(canvas, "◀", Vector2(112, -82), "left")
	_add_hold_button(canvas, "▶", Vector2(182, -82), "right")

	var buy := Button.new()
	buy.text = "BUY EXPANSION  $250"
	buy.position = Vector2(-255, -92)
	buy.size = Vector2(230, 58)
	buy.anchor_left = 1.0
	buy.anchor_right = 1.0
	buy.anchor_top = 1.0
	buy.anchor_bottom = 1.0
	buy.add_theme_font_size_override("font_size", 17)
	buy.pressed.connect(_buy_expansion)
	canvas.add_child(buy)

func _add_hold_button(canvas: CanvasLayer, label_text: String, offset: Vector2, direction: String) -> void:
	var button := Button.new()
	button.text = label_text
	button.size = Vector2(62, 62)
	button.position = Vector2(24 + offset.x, 0)
	button.anchor_top = 1.0
	button.anchor_bottom = 1.0
	button.position.y = offset.y
	button.add_theme_font_size_override("font_size", 25)
	match direction:
		"forward":
			button.position.x = 28
			button.button_down.connect(func(): hold_forward = true)
			button.button_up.connect(func(): hold_forward = false)
		"back":
			button.position.x = 28
			button.button_down.connect(func(): hold_back = true)
			button.button_up.connect(func(): hold_back = false)
		"left":
			button.position.x = 98
			button.button_down.connect(func(): hold_left = true)
			button.button_up.connect(func(): hold_left = false)
		"right":
			button.position.x = 168
			button.button_down.connect(func(): hold_right = true)
			button.button_up.connect(func(): hold_right = false)
	canvas.add_child(button)

func _buy_expansion() -> void:
	if cash < 250:
		status_label.text = "Not enough cash yet. Your shop earns money every second."
		return
	cash -= 250
	owned_buildings += 1
	income_per_second += 5
	var x := 17.0 + float(owned_buildings) * 7.0
	_make_box("Business expansion %d" % owned_buildings, Vector3(x, 1.5, -2), Vector3(5.2, 3.0, 4.0), Color(0.16, 0.58, 0.38))
	_make_box("Expansion roof %d" % owned_buildings, Vector3(x, 3.1, -2), Vector3(5.6, 0.2, 4.4), Color(0.08, 0.20, 0.16))
	status_label.text = "Business expanded! Income is now $%d/sec." % income_per_second
	_update_hud()

func _update_hud() -> void:
	if cash_label:
		cash_label.text = "Cash: $%d   |   Income: $%d/sec" % [cash, income_per_second]
