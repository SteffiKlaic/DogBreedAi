import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(private http: HttpClient, private cdr: ChangeDetectorRef)
  {
  }

  imageUrl: string | null = null;
  uploadInfo: any = null;

  onFileSelected(event: Event) {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];

  if (!file) {
    return;
  }

  this.imageUrl = URL.createObjectURL(file);

  const formData = new FormData();
  formData.append('file', file);

  this.http.post(
    'http://localhost:5072/api/dogs/upload',
    formData
  ).subscribe(response => {
    this.uploadInfo = response;
    this.cdr.detectChanges();
  });
}
}
