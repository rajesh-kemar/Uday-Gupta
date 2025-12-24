import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService, UserRequest, UserResponse } from '../../core/services/user.service';
import { NavbarComponent } from '../../shared/navbar/navbar.component';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UsersComponent implements OnInit {

  userName = '';
  rolesFromToken: string[] = [];

  allUsers: UserResponse[] = [];
  users: UserResponse[] = [];
  loadingUsers = true;

  searchText = '';

  page = 1;
  pageSize = 5;
  pageSizeOptions = [5, 10, 20];

  showAddForm = false;
  isEditMode = false;
  isSubmitting = false;

  showPassword = false;

  formMessage = '';
  formError = false;

  user = {
    userId: undefined as number | undefined,
    username: '',
    email: '',
    password: '',
    role: ''
  };

  roles = ['Admin', 'WarehouseStaff', 'TransportAdmin', 'DeliveryStaff'];

  roleMap: Record<string, number> = {
    Admin: 1,
    WarehouseStaff: 2,
    TransportAdmin: 3,
    DeliveryStaff: 4
  };

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.userName = this.authService.getUserName();
    this.rolesFromToken = this.authService.getRoles();
    this.loadUsers();
  }

  // ===== LOAD =====
  loadUsers(): void {
    this.loadingUsers = true;
    this.userService.getUsers().subscribe(res => {
      this.allUsers = res ?? [];
      this.page = 1;
      this.applyPagination();
      this.loadingUsers = false;
      this.cdr.detectChanges();
    });
  }

  // ===== SINGLE SEARCH (FIXED) =====
  searchUsers(): void {
    const text = this.searchText.trim();
    if (!text) {
      this.loadUsers();
      return;
    }

    const filter: any = {};

    if (this.roles.includes(text)) {
      filter.role = text;
    } else if (text.includes('@')) {
      filter.email = text;
    } else {
      filter.username = text;
    }

    this.loadingUsers = true;
    this.userService.filterUsers(filter).subscribe(res => {
      this.allUsers = res ?? [];
      this.page = 1;
      this.applyPagination();
      this.loadingUsers = false;
      this.cdr.detectChanges();
    });
  }

  // ===== PAGINATION =====
  applyPagination(): void {
    const start = (this.page - 1) * this.pageSize;
    this.users = this.allUsers.slice(start, start + this.pageSize);
  }

  nextPage() {
    if (this.page * this.pageSize < this.allUsers.length) {
      this.page++;
      this.applyPagination();
    }
  }

  prevPage() {
    if (this.page > 1) {
      this.page--;
      this.applyPagination();
    }
  }

  changePageSize() {
    this.page = 1;
    this.applyPagination();
  }

  // ===== ADD =====
openAdd() {
  this.isEditMode = false;
  this.showPassword = false;
  this.formMessage = '';
  
  this.user = {
    userId: undefined,
    username: '',
    email: '',
    password: '',
    role: ''
  };

  this.showAddForm = true;
}


  // ===== EDIT =====
  editUser(u: UserResponse) {
    this.isEditMode = true;
    this.showPassword = false;
    this.user = {
      userId: u.userId,
      username: u.username,
      email: u.email,
      password: '',
      role: u.role
    };
    this.showAddForm = true;
  }

  // ===== VALIDATION =====
  private validateForm(): boolean {
    if (!this.user.username.trim()) return this.setError('Username required');
    if (!this.user.email.trim()) return this.setError('Email required');

    if (!this.isEditMode && !this.user.password)
      return this.setError('Password required');

    if (this.user.password && this.user.password.length < 6)
      return this.setError('Password must be at least 6 characters');

    if (!this.user.role) return this.setError('Role required');

    return true;
  }

  private setError(msg: string): boolean {
    this.formError = true;
    this.formMessage = msg;
    return false;
  }

  // ===== SAVE =====
  submitForm(): void {
    if (!this.validateForm()) return;

    this.isSubmitting = true;

    const payload: UserRequest = {
      userId: this.user.userId,
      username: this.user.username.trim(),
      email: this.user.email.trim().toLowerCase(),
      role: this.user.role,
      roleIds: [this.roleMap[this.user.role]]
    };

    if (this.user.password) payload.password = this.user.password;

    this.userService.addOrUpdateUser(payload).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.showAddForm = false;
        this.loadUsers();
      },
      error: err => {
        this.isSubmitting = false;
        this.formError = true;
        this.formMessage = err?.error?.message || 'Save failed';
      }
    });
  }

  deleteUser(id: number) {
    if (!confirm('Delete user?')) return;
    this.userService.deleteUser(id).subscribe(() => this.loadUsers());
  }

  closeForm() {
    this.showAddForm = false;
  }
}
