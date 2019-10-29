import React, { Component } from 'react';
import {Icon} from "../components/icon/index";
import styles from './error.css';

export const permissions = {
  VIEW_MATERIAL_PROPERTIES: 'View Material properties',
  VIEW_MATERIAL_RATE_HISTORY: 'View Material Rate History',
  VIEW_SERVICE_PROPERTIES: 'View Service properties',
  VIEW_SERVICE_RATE_HISTORY: 'View Service Rate History',
  VIEW_SFG_PROPERTIES: 'View SFG properties',
  VIEW_PACKAGE_PROPERTIES: 'View Package properties',
  VIEW_CPR_VALUES: 'View CPR values',
  VIEW_CURRENT_EXCHANGE_RATE: 'View current Exchange rate',
  EDIT_MATERIAL_PROPERTIES: 'Edit Material properties',
  SETUP_CPR_VERSION: 'Setup CRP Version',
  CREATE_MATERIAL_PROPERTIES: 'Create Material properties',
  CREATE_SERVICE_PROPERTIES: 'Create Service properties',
  SETUP_MATERIAL_RATE_VERSIONS: 'Setup Material Rate versions',
  SETUP_MATERIAL_RENTAL_RATE_VERSIONS: 'Setup Material Rental Rate versions',
  SETUP_SERVICE_RATE_VERSIONS: 'Setup Service Rate versions',
  VIEW_RENTAL_RATE_HISTORY: 'View Rental Rate History',
  EDIT_SERVICE_PROPERTIES: 'Edit Service properties',
  EDIT_SFG_PROPERTIES: 'Edit SFG properties',
  CREATE_SFG: 'Create SFG',
  CREATE_PACKAGE: 'Create Package',
  EDIT_PACKAGE_PROPERTIES: 'Edit Package properties',
  SETUP_EXCHANGE_RATE_VERSION_AND_VIEW_EXCHANGE_RATE_HISTORY: 'Setup Exchange Rate version and View Exchange rate history',
  MATERIAL_BULK_RATES_VIEW: 'Material Bulk Rates View',
  SERVICE_BULK_RATES_VIEW: 'Service Bulk Rates View',
  MATERIAL_BULK_RATES_EDIT: 'Material Bulk Rates Edit',
  SERVICE_BULK_RATES_EDIT: 'Service Bulk Rates Edit'
};

export const Permissioned = (WrappedComponent, allowedPermissions, hasAll = false, AlternateView, isAlternateViewRequired = true) => {
  let PermissionedComponent = class extends Component {
    constructor(props, context) {
      super(props, context);
      this.state = {
        allowedPermissions: props.allowedPermissions || (allowedPermissions ? (typeof allowedPermissions === "function" ? allowedPermissions(props) : allowedPermissions) :  []),
        allPermissions:window.userPermissions || [],
        alternateView: props.alternateView || AlternateView || ErrorScreen
      };
    }

    componentWillReceiveProps(nextProps) {
      nextProps.allowedPermissions && this.setState({ allowedPermissions: nextProps.allowedPermissions });
    }
    componentWillUpdate(props,state)
    {
      this.state.allPermissions = state.allPermissions || Object.keys(permissions).map(key=>permissions[key]);
    }
    hasPermission(and)
    {
      let hasPermissionTo = true;
      let filteredList =  this.state.allPermissions.filter(permission =>{
        return this.state.allowedPermissions.filter(allowedPermission =>{
            return allowedPermission === permission;
          }).length > 0;
      });
      hasPermissionTo = and ? (filteredList.length  === this.state.allowedPermissions.length) : (filteredList.length > 0);
      return hasPermissionTo;
    }
    render() {
      const AlternateView = this.state.alternateView;
      if (this.hasPermission(hasAll)) {
        return (<WrappedComponent {...this.props} />);}
      return isAlternateViewRequired ? <AlternateView/> : <div/>;
    }
  };
  return PermissionedComponent;
};

export class PermissionedForNonComponents extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      allPermissions: window.userPermissions || []
    }
  }
  hasPermission()
  {
    let hasPermissionTo = true;
    let filteredList =  this.state.allPermissions.filter(permission =>{
      return this.props.allowedPermissions.filter(allowedPermission =>{
          return allowedPermission === permission;
        }).length > 0;
    });
    hasPermissionTo = (filteredList.length  === this.props.allowedPermissions.length);
    return hasPermissionTo;
  }
  render() {
    if(this.hasPermission()) {
      return this.props.children;
    }
    else
      return <div/>;
  }
}

export class ErrorScreen extends React.Component {
  render() {
    return <div className={styles.error}>
      <span className={styles.text}> Sorry, you do not have permissions to view this page</span>
      <Icon name="notAllowed" className={styles.icon}/>
    </div>;
  }
}
