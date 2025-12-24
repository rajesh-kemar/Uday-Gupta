import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../shared/navbar/navbar.component';
import { ProductService, ProductRequest, ProductResponse } from '../../core/services/product.service';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit {

  userName = '';
  roles: string[] = [];

  allProducts: ProductResponse[] = [];
  products: ProductResponse[] = [];
  originalProducts: ProductResponse[] = [];

  searchText = '';
  searchBy: 'name' | 'weight' = 'name';
  loadingProducts = true;

  showAddForm = false;
  isEditMode = false;
  isSubmitting = false;
  formError = false;
  formMessage = '';

  page = 1;
  pageSize = 5;
  pageSizeOptions = [5, 10, 20];

  product: ProductRequest = {
    productId: undefined,
    name: '',
    description: '',
    weight: 0
  };

  constructor(private productService: ProductService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadUser();
    this.loadProducts();
  }

  private loadUser(): void {
    this.userName = localStorage.getItem('userName') || 'User';
    const rolesStr = localStorage.getItem('roles');
    this.roles = rolesStr ? JSON.parse(rolesStr) : [];
  }

  loadProducts(): void {
    this.loadingProducts = true;
    this.productService.getAll().subscribe(res => {
      this.allProducts = res || [];
      this.originalProducts = [...this.allProducts];
      this.page = 1;
      this.applyPagination();
      this.loadingProducts = false;
      this.cdr.detectChanges();
    });
  }

  searchProducts(): void {
    const text = this.searchText.trim().toLowerCase();
    if (!text) {
      this.allProducts = [...this.originalProducts];
      this.page = 1;
      this.applyPagination();
      return;
    }

    let filtered: ProductResponse[] = [];

    switch (this.searchBy) {
      case 'name':
        filtered = this.originalProducts.filter(p =>
          p.name.toLowerCase().includes(text)
        );
        break;
      case 'weight':
        filtered = this.originalProducts.filter(p =>
          p.weight.toString().includes(text)
        );
        break;
    }

    this.allProducts = filtered;
    this.page = 1;
    this.applyPagination();
  }

  applyPagination(): void {
    const start = (this.page - 1) * this.pageSize;
    this.products = this.allProducts.slice(start, start + this.pageSize);
  }

  nextPage(): void {
    if (this.page * this.pageSize < this.allProducts.length) {
      this.page++;
      this.applyPagination();
    }
  }

  prevPage(): void {
    if (this.page > 1) {
      this.page--;
      this.applyPagination();
    }
  }

  changePageSize(): void {
    this.page = 1;
    this.applyPagination();
  }

  openAdd(): void {
    this.isEditMode = false;
    this.formError = false;
    this.formMessage = '';
    this.product = { productId: undefined, name: '', description: '', weight: 0 };
    this.showAddForm = true;
  }

  editProduct(p: ProductResponse): void {
    this.isEditMode = true;
    this.formError = false;
    this.formMessage = '';
    this.product = { ...p };
    this.showAddForm = true;
  }

  closeForm(): void {
    if (!this.isSubmitting) this.showAddForm = false;
  }

  private validateForm(): boolean {
    if (!this.product.name?.trim()) return this.setError('Name required');
    if (this.product.weight <= 0) return this.setError('Weight must be greater than 0');
    return true;
  }

  private setError(msg: string): boolean {
    this.formError = true;
    this.formMessage = msg;
    return false;
  }

  submitForm(): void {
    if (!this.validateForm()) return;
    this.isSubmitting = true;
    this.productService.addOrUpdate(this.product).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.showAddForm = false;
        this.loadProducts();
      },
      error: err => {
        this.isSubmitting = false;
        this.formError = true;
        this.formMessage = err?.error?.message || 'Save failed';
      }
    });
  }

  deleteProduct(id: number): void {
    if (!confirm('Delete this product?')) return;
    this.productService.delete(id).subscribe(() => {
      this.loadProducts();
    });
  }
}
