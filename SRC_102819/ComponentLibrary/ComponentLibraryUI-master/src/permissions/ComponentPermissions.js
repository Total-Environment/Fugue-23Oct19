import {permissions} from "./permissions";

export function getViewRentalRatePermission() {
  return [permissions.VIEW_RENTAL_RATE_HISTORY];
}

export function getComponentRateHistoryPermissions(componentType) {
  switch (componentType) {
    case 'material':
      return [permissions.VIEW_MATERIAL_RATE_HISTORY];
    case 'service':
      return [permissions.VIEW_SERVICE_RATE_HISTORY];
    default:
      return [];
  }
}

export function getComponentViewPermissions(componentType) {
  switch (componentType) {
    case 'material':
      return [permissions.VIEW_MATERIAL_PROPERTIES];
    case 'service':
      return [permissions.VIEW_SERVICE_PROPERTIES];
    case 'sfg':
      return [permissions.VIEW_SFG_PROPERTIES];
    case 'package':
      return [permissions.VIEW_PACKAGE_PROPERTIES];
    default:
      return [];
  }
}

export function getCreateComponentPermissions(props) {
  switch (props.componentType) {
    case 'material':
      return [permissions.CREATE_MATERIAL_PROPERTIES];
    case 'service':
      return [permissions.CREATE_SERVICE_PROPERTIES];
    default:
      return [];
  }
}

export function getEditComponentPermissions(props) {
  switch (props.componentType || props.route.componentType) {
    case 'material':
      return [permissions.EDIT_MATERIAL_PROPERTIES];
    case 'service':
      return [permissions.EDIT_SERVICE_PROPERTIES];
    case 'sfg':
      return [permissions.EDIT_SFG_PROPERTIES];
    case 'package':
      return [permissions.EDIT_PACKAGE_PROPERTIES];
    default:
      return [];
  }
}

export function getViewComponentRateHistoryPermissions(props) {
  switch(props.route.componentType) {
    case 'material':
      return [permissions.VIEW_MATERIAL_RATE_HISTORY];
    case 'service':
      return [permissions.VIEW_SERVICE_RATE_HISTORY];
    default:
      return [];
  }
}

export function getCompositeComponentsPermissions(props) {
  switch (props.route.componentType){
    case 'sfg':
      switch (props.route.mode) {
        case 'edit':
          return [permissions.EDIT_SFG_PROPERTIES];
        case 'create':
          return [permissions.CREATE_SFG];
        default:
          return [];
      }
    case 'package':
      switch (props.route.mode) {
        case 'edit':
          return [permissions.EDIT_PACKAGE_PROPERTIES];
        case 'create':
          return [permissions.CREATE_PACKAGE];
        default:
          return [];
      }
  }
}

export function getBrandPermissions(props) {
  switch (props.params.brandCode === 'new' ? 'create' : props.params.action === 'edit' ? 'edit' : '') {
    case 'edit':
      return [permissions.EDIT_MATERIAL_PROPERTIES];
    case 'create':
      return [permissions.CREATE_MATERIAL_PROPERTIES];
    default:
      return [permissions.VIEW_MATERIAL_PROPERTIES];
  }
}

export function getViewCPRValuesPermissions() {
  return [permissions.VIEW_CPR_VALUES];
}

export function getBulkRatesViewPermissions(props) {
  switch (props.route.componentType) {
    case 'material':
      return [permissions.MATERIAL_BULK_RATES_VIEW];
    case 'service':
      return [permissions.SERVICE_BULK_RATES_VIEW];
    default:
      return [];
  }
}

export function getBulkRatesEditPermissions(componentType) {
  switch (componentType) {
    case 'material':
      return [permissions.MATERIAL_BULK_RATES_EDIT];
    case 'service':
      return [permissions.SERVICE_BULK_RATES_EDIT];
    default:
      return [];
  }
}

export function getViewExchangeRatePermissions() {
  return [permissions.VIEW_CURRENT_EXCHANGE_RATE];
}

export function getViewExchangeRateHistoryPermissions() {
  return [permissions.SETUP_EXCHANGE_RATE_VERSION_AND_VIEW_EXCHANGE_RATE_HISTORY];
}

export function getCreateCPRPermissions() {
  return [permissions.SETUP_CPR_VERSION, permissions.VIEW_CPR_VALUES];
}


