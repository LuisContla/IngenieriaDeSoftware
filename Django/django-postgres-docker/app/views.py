from django.contrib.auth.forms import UserCreationForm
from django.contrib.auth.models import User
from django.contrib.auth.decorators import user_passes_test
from django.shortcuts import render, redirect, get_object_or_404
from django.http import HttpResponse
from django.shortcuts import render, redirect
from django.contrib import messages
from .forms import RegistroForm, LoginForm
from .models import Usuario, Libro
from functools import wraps
from django.shortcuts import redirect

def login_requerido(vista_func):
    @wraps(vista_func)
    def wrapper(request, *args, **kwargs):
        if not request.session.get('usuario_id'):
            return redirect('login')
        return vista_func(request, *args, **kwargs)
    return wrapper

def admin_requerido(vista_func):
    @wraps(vista_func)
    def wrapper(request, *args, **kwargs):
        usuario_id = request.session.get('usuario_id')
        if not usuario_id:
            return redirect('login')
        usuario = Usuario.objects.get(id=usuario_id)
        if usuario.rol != 1:
            messages.error(request, "No tienes permisos para acceder a esta página.")
            return redirect('index')
        return vista_func(request, *args, **kwargs)
    return wrapper

@login_requerido
def index(request):
    usuario = Usuario.objects.get(id=request.session['usuario_id'])
    return render(request, 'index.html', {'usuario': usuario})

def logout(request):
    request.session.flush()
    return redirect('login')

def registro(request):
    if request.method == 'POST':
        print("Formulario recibido")  # <-- Esto debe aparecer en la terminal
        nombre = request.POST.get('nombre')
        correo = request.POST.get('correo')
        contraseña = request.POST.get('contraseña')
        confirmar_contraseña = request.POST.get('confirmar_contraseña')
        acepta_terminos = request.POST.get('acepta_terminos') == 'on'

        if not acepta_terminos:
            messages.error(request, "Debes aceptar los términos y condiciones.")
        elif contraseña != confirmar_contraseña:
            messages.error(request, "Las contraseñas no coinciden.")
        elif Usuario.objects.filter(correo=correo).exists():
            messages.error(request, "El correo ya está registrado.")
        else:
            Usuario.objects.create(
                nombre=nombre,
                correo=correo,
                contraseña=contraseña,
                acepta_terminos=acepta_terminos
            )
            messages.success(request, "Usuario registrado correctamente.")
            return redirect('login')  # Asegúrate que 'login' es el nombre de tu url de login
    return render(request, 'registro.html')

def login(request):
    if request.method == 'POST':
        correo = request.POST.get('correo')
        contraseña = request.POST.get('contraseña')
        try:
            usuario = Usuario.objects.get(correo=correo, contraseña=contraseña)
            request.session['usuario_id'] = usuario.id
            messages.success(request, f"Bienvenido, {usuario.nombre}!")
            return redirect('index')  # Asegúrate que 'index' es el nombre de tu url de inicio
        except Usuario.DoesNotExist:
            messages.error(request, "Correo o contraseña incorrectos.")
    return render(request, 'login.html')

@login_requerido
@admin_requerido
def dashboard(request):
    usuario = Usuario.objects.get(id=request.session['usuario_id'])
    usuarios = Usuario.objects.all()
    admins = usuarios.filter(rol=1)
    usuarios_normales = usuarios.filter(rol=0)
    return render(request, 'usuarios_dashboard.html', {
        'usuario': usuario,
        'usuarios': usuarios,
        'admins': admins,
        'usuarios_normales': usuarios_normales,
    })

@login_requerido
def libros(request):
    usuario = Usuario.objects.get(id=request.session['usuario_id'])
    libros = Libro.objects.all()
    return render(request, 'libros_dashboard.html', {'usuario': usuario, 'libros': libros})

@login_requerido
@admin_requerido
def registrar_usuario(request):
    if request.method == 'POST':
        nombre = request.POST.get('nombre')
        correo = request.POST.get('correo')
        contraseña = request.POST.get('contraseña')
        rol = int(request.POST.get('rol', 0))
        if Usuario.objects.filter(correo=correo).exists():
            messages.error(request, "El correo ya está registrado.")
        else:
            Usuario.objects.create(
                nombre=nombre,
                correo=correo,
                contraseña=contraseña,
                rol=rol
            )
            messages.success(request, "Usuario registrado correctamente.")
            return redirect('dashboard')
    return render(request, 'registrar_usuario.html')

# Vistas de Administradores

# def es_admin(user):
#     return user.is_superuser

# @user_passes_test(es_admin)
# def dashboard_usuarios(request):
#     usuarios = User.objects.all()
#     return render(request, 'usuarios_dashboard.html', {'usuarios': usuarios})

# @user_passes_test(es_admin)
# def eliminar_usuario(request, user_id):
#     usuario = get_object_or_404(User, id=user_id)
#     usuario.delete()
#     return redirect('dashboard_usuarios')

# @user_passes_test(es_admin)
# def crear_usuario(request):
#     # Aquí iría un formulario personalizado
#     return render(request, 'crear_usuario.html')

# @user_passes_test(es_admin)
# def editar_usuario(request, user_id):
#     # Aquí iría la lógica de edición
#     return render(request, 'editar_usuario.html')
