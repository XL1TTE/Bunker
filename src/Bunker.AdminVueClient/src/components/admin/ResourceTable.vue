<script setup lang="ts" generic="T">
import styles from '@/components/admin/resource-table.module.css';

export interface ResourceColumn<T> {
  label: string;
  value: (row: T) => string;
  // Optional column width hint (e.g. '1fr', '120px').
  width?: string;
}

withDefaults(
  defineProps<{
    columns: ResourceColumn<T>[];
    rows: T[];
    rowKey: (row: T) => string;
    loading?: boolean;
    emptyText?: string;
    canEdit?: boolean;
    canDelete?: boolean;
  }>(),
  {
    loading: false,
    emptyText: 'Nothing here yet.',
    canEdit: true,
    canDelete: true,
  },
);

defineEmits<{ edit: [row: T]; delete: [row: T] }>();
</script>

<template>
  <div :class="styles.scroll">
    <table :class="styles.table">
      <thead>
        <tr>
          <th
            v-for="col in columns"
            :key="col.label"
            :style="col.width ? { width: col.width } : undefined"
          >
            {{ col.label }}
          </th>
          <th v-if="canEdit || canDelete" :class="styles.actionsHead"><span class="visually-hidden">Actions</span></th>
        </tr>
      </thead>
      <tbody>
        <tr v-if="loading">
          <td v-for="i in columns.length + (canEdit || canDelete ? 1 : 0)" :key="i" :class="styles.skeletonCell">
            <span :class="styles.skeletonBar" />
          </td>
        </tr>
        <tr v-else-if="rows.length === 0">
          <td :colspan="columns.length + (canEdit || canDelete ? 1 : 0)" :class="styles.emptyCell">
            {{ emptyText }}
          </td>
        </tr>
        <tr v-for="row in rows" :key="rowKey(row)">
          <td v-for="col in columns" :key="col.label">{{ col.value(row) }}</td>
          <td v-if="canEdit || canDelete" :class="styles.actionsCell">
            <button v-if="canEdit" type="button" class="btn btnGhost btnSm" @click="$emit('edit', row)">Edit</button>
            <button v-if="canDelete" type="button" class="btn btnDanger btnSm" @click="$emit('delete', row)">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>