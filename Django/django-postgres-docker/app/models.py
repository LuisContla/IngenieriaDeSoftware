from django.db import models

class Libro(models.Model):
    nombre = models.CharField(max_length=100)
    # portada = models.ImageField(upload_to='portadas/', blank=True, null=True)
    autor = models.CharField(max_length=100)
    descripcion = models.TextField(blank=True)

    def __str__(self):
        return self.nombre

class Usuario(models.Model):
    rol = models.IntegerField(default=0)  # 0 = usuario, 1 = admin
    nombre = models.CharField(max_length=100)
    correo = models.EmailField(unique=True)
    contraseña = models.CharField(max_length=128)
    favoritos = models.ManyToManyField(Libro, blank=True, related_name='usuarios_favoritos')
    acepta_terminos = models.BooleanField(default=False)  # <-- Agrega esta línea

    def __str__(self):
        return self.nombre