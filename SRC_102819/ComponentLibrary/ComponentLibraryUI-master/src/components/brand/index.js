import React from "react";
import brandStyles from './index.css';
import styles from '../component/index.css'
import { ComponentStatus } from "../component-status/index";
import { head, col, idFor, preventClickPropagation } from "../../helpers";
import Collapse from 'rc-collapse';
import { Icon } from "../icon/index";
import { Loading } from "../loading/index";
import { Link } from "react-router";
import { AutoAffix } from "react-overlays";
import DataType from '../data-types';
import { validate } from '../../validator';
import { CreateBrand } from '../create-brand/index';
import { ConfirmationDialog } from '../confirmation-dialog';
import { AlertDialog } from '../alert-dialog';
import { button } from '../../css-common/forms.css';
import { browserHistory } from 'react-router';
import R from 'ramda';
import {PermissionedForNonComponents} from "../../permissions/permissions";
import {getBrandPermissions} from "../../permissions/ComponentPermissions";

export class Brand extends React.Component {

  constructor(props) {
    super(props);
    this.initialState = {
      openCreateForm: props.brandCode === 'new',
      activePanel: props.brandCode,
      newBrandFormPanel: 'new',
      details: props.details,
      brandList: [],
      error: {
        message: '',
        shown: false
      },
      confirmation: {
        message: '',
        shown: false
      }
    };

    this.state = Object.assign({}, this.initialState);

    this.handleAddBrand = this.handleAddBrand.bind(this);
    this.handleEditBrand = this.handleEditBrand.bind(this);
    this.resetView = this.resetView.bind(this);
    this.cancelErrorDialog = this.cancelErrorDialog.bind(this);
    this.yesConfirmationDialog = this.yesConfirmationDialog.bind(this);
    this.showConfirmationDialog = this.showConfirmationDialog.bind(this);
    this.noConfirmationDialog = this.noConfirmationDialog.bind(this);
    this.ensureMasterDataIsPresent = this.ensureMasterDataIsPresent.bind(this);
    this.setBrandDataTypeDetails = this.setBrandDataTypeDetails.bind(this);
  }



  isFormValid() {
    let updatedState = Object.assign(
      {},
      this.state,
      {
        brandList: this.state.brandList.map(c => {
          return Object.assign({}, {
            columns: c.columns.map(b => {
              return Object.assign({}, b, {
                validity: validate(b.value, b, b.name)
              })
            })
          });
        })
      });

    const isValid = updatedState.brandList.reduce((acc, brand) => brand.columns.reduce((c, col) => col.validity.isValid && c, true) && acc, true);

    this.setState(updatedState);

    return isValid;
  }

  handleEditBrand(e) {
    e.preventDefault();
    let updatedDetails = JSON.parse(JSON.stringify(this.state.details));
    if (!this.isFormValid()) {
      this.showErrorDialog("There's an error in the form. Please fix it and submit.");
      return;
    }
    this.props.onAddOrEditBrand(this.props.materialCode, this.updateBrandData(updatedDetails, this.state.brandList), true);
  }

  updateBrandData(details, brandsList) {
    details.headers = details.headers.map(header => {
      if (header.key === 'purchase') {
        return Object.assign({}, header, {
          columns: header.columns.map(column => {
            if (column.key === 'approved_brands') {
              return Object.assign({}, column, {
                value: brandsList
              });
            }
            return column;
          })
        });
      }
      return header;
    });
    return details;
  }

  handleAddBrand(newBrand) {
    let _self = this;
    let newDetails = JSON.parse(JSON.stringify(this.props.details));
    let brands = head(newDetails, 'purchase', 'approved_brands', 'value') || [];
    brands.push({ columns: newBrand });
    this.props.onAddOrEditBrand(this.props.materialCode, _self.updateBrandData(newDetails, brands));
  }

  showErrorDialog(message) {
    this.setState({ error: { message, shown: true } });
  }

  cancelErrorDialog() {
    this.setState({ error: { shown: false } });
  }

  noConfirmationDialog() {
    this.setState({ confirmation: { shown: false } });
  }

  showConfirmationDialog(message) {
    this.setState({ confirmation: { message, shown: true } });
  }

  yesConfirmationDialog() {
    this.setState({ confirmation: { shown: false } });
    browserHistory.push(`/materials/${this.props.materialCode}/brands/${this.props.brandCode}`);
  }

  resetView(opts) {
    this.setState(Object.assign({}, { openCreateForm: false, activePanel: this.props.brandCode }, opts || {}));
  }

  setBrandDataTypeDetails(brandDefinition, brandList) {
    brandList = brandList || [];
    this.setState({
      brandList: brandList.map(c => {
        return Object.assign({}, {
          columns: c.columns.map(b => {
            let defColumn = brandDefinition.columns.find(d => d.key === b.key);
            if (defColumn) {
              return Object.assign({}, b, {
                isRequired: defColumn.isRequired || false,
                isSearchable: defColumn.isSearchable || false
              })
            }
            return b;
          })
        })
      })
    });
  }

  componentWillReceiveProps(nextProps) {
    let err = nextProps.newBrandError || '';
    this.ensureMasterDataIsPresent(nextProps);
    if (err) {
      return this.showErrorDialog(err.message || err);
    }

    if (nextProps.addedBrand) {
      const brands = head(nextProps.details, 'purchase', 'approved_brands', 'value') || [];
      const latestBrand = brands[brands.length - 1] || {};
      const brandCode = (R.head(latestBrand.columns) || {}).value;
      return window.location.href = (`/materials/${nextProps.materialCode}/brands/${brandCode}`);
    }

    if (nextProps.brandCode === 'new') {
      this.setState({ openCreateForm: true, activePanel: this.state.newBrandFormPanel });
    }

    if (nextProps.action === 'edit') {
      if (nextProps.updatedBrand) {
        return window.location.href = (`/materials/${nextProps.materialCode}/brands/${nextProps.brandCode}`);
      }
      this.resetView({ activePanel: nextProps.brandCode });
    }
    this.setState({ details: nextProps.details, brandList: head(nextProps.details, 'purchase', 'approved_brands', 'value') || [] });
    if (nextProps.brandDefinition && nextProps.details) {
      this.setBrandDataTypeDetails(nextProps.brandDefinition, head(nextProps.details, 'purchase', 'approved_brands', 'value'));
    }
  }

  componentDidMount() {
    if (!this.props.details) {
      this.props.onBrandFetchRequest(this.props.materialCode);
    }
    if (!this.props.brandDefinition) {
      this.props.onBrandDefinitionFetchRequest();
    }

    this.ensureMasterDataIsPresent(this.props);
  }

  renderImage() {
    const imageUrl = head(this.props.details, 'general', 'image', 'value', 0, 'url');
    return imageUrl ? <a className={styles.headerImage} target="_blank" href={imageUrl}
      style={{ backgroundImage: `url(${imageUrl})` }} /> : '';
  }

  onChangeValue(column, value) {
    let updatedState = Object.assign({}, this.state);
    let brandUnderEditCode = this.props.brandCode;

    updatedState.brandList = updatedState.brandList.map(x => {
      let brandCode = x.columns.find(c => c.key === 'brand_code').value || '';
      if (brandCode !== brandUnderEditCode) {
        return x;
      }
      return Object.assign({}, x, {
        columns: x.columns.map(y => {
          if (y.key === column.key) {
            return Object.assign({}, y, { value });
          }
          return y;
        })
      })
    });

    this.setState(updatedState);

  }

  ensureMasterDataIsPresent(props) {
    // Populate MasterData if it is ID
    let brandDefinition = (props.brandDefinition && props.brandDefinition.columns) || [];
    let masterData = props.masterData || {};
    brandDefinition
      .map(x => x.dataType)
      .filter(x => x.name === 'MasterData')
      .forEach(masterDataType => {
        if (!masterData[masterDataType.subType]) {
          return props.onMasterDataFetch(masterDataType.subType);
        }
      });
  }

  renderColumn(column, brandCode) {
    let group = this.props.details && this.props.details.group;
    let isEditMode = this.props.action === 'edit' && brandCode === this.props.brandCode;
    if (isEditMode) {
      if (column.dataType.name === 'MasterData') {
        column.dataType.values = this.props.masterData[column.dataType.subType] || { values: [], status: 'blank' };
      }
      column.editable = true;
      column.validity = validate(column.value, column, column.name);
    } else {
      column.editable = false;
    }
    return <div key={column.name} className={styles.column}>
      <dt id={idFor(name, 'title')} className={styles.columnTitle}>{column.name} <abbr className={styles.requiredMarker}>{isEditMode && column.isRequired ? '*' : ''}</abbr></dt>
      <DataType columnValue={column} componentType={"brand"} group={group} columnName={column.name} onChange={value => this.onChangeValue(column, value)} />
    </div>
  }



  renderBrandNewHeader(headerName) {
    let group = this.props.details && this.props.details.group;
    return <Collapse.Panel key={this.state.newBrandFormPanel} showArrow={false} header={
      <span id={idFor(this.state.newBrandFormPanel)}>
        <Icon name="arrow" className="arrow" />{headerName}
      </span>
    } style={{ display: this.state.openCreateForm ? 'block' : 'none' }}>
      <CreateBrand
        group={group}
        doneAdding={this.props.addedBrand} isAddingBrand={this.props.brandAdding || false}
        brandDefinition={this.props.brandDefinition}
        onMasterDataFetch={this.props.onMasterDataFetch}
        masterData={this.props.masterData}
        onAddBrand={this.handleAddBrand}
        onCancel={this.resetView} />
    </Collapse.Panel>
  }

  renderHeader(headerName, { columns }) {
    let brandCode = (columns.find(c => c.key === 'brand_code') || {}).value;
    return (<Collapse.Panel key={headerName} showArrow={false} header={<span id={idFor(headerName)}>
      <Icon name="arrow" className="arrow" />{headerName}
      <PermissionedForNonComponents allowedPermissions={getBrandPermissions({params: {action: 'edit'}})}>
      <span className={brandStyles.edit}>
        <Link to={`/materials/${this.props.materialCode}/brands/${brandCode}/edit`}>Edit</Link>
      </span>
      </PermissionedForNonComponents>
    </span>}>
      <div className={styles.columnsRow}>
        {
          columns
            .filter(column => column.key !== 'image' || this.props.editable)
            .map(column => this.renderColumn(column, brandCode))
        }
        {this.props.action === 'edit' && brandCode === this.props.brandCode ? <div className={brandStyles.action}>
          <input id="edit-button"
            className={button}
            type="button"
            onClick={this.handleEditBrand}
            value={this.props.isSavingBrand ? 'Updating ...' : 'Update'}
            disabled={this.props.isSavingBrand} />
          <a id="cancel-button" className={button} onClick={this.showConfirmationDialog}>Cancel</a>
        </div> : ''}
      </div>
    </Collapse.Panel>);
  }

  renderAddNewBrandButton() {
    return this.state.openCreateForm ? '' : <button id="addNewBrand" className={brandStyles.addNewButton}
      onClick={() => this.props.action !== 'edit' ?
        this.setState({ openCreateForm: true, activePanel: this.state.newBrandFormPanel })
        : browserHistory.push(`/materials/${this.props.materialCode}/brands/new`)}>
      Add New Brand
    </button>;
  }

  render() {
    if (this.state.brandList && this.props.brandDefinition) {
      return (
        <div>
          <div className={styles.headerAffixed}>
            <header className={brandStyles.header}>
              <a className={brandStyles.backToComponent} href={`/materials/${this.props.materialCode}`}>
                Back to material details</a>
              <h3 className={brandStyles.title}>Associated Brands</h3>
              <div className={styles.left}>
                {this.renderImage()}
                <div className={styles.titleAndStatus}>
                  <h2 id={idFor('title')} className={brandStyles.titleHeader}>
                    {head(this.props.details, 'general', 'material_name', 'value')} | {
                      head(this.props.details, 'general', 'material_code', 'value')}</h2>
                  <div className={brandStyles.tagItem}>
                    <ComponentStatus
                      value={head(this.props.details, 'general', 'material_status', 'value')} />
                    <PermissionedForNonComponents allowedPermissions={getBrandPermissions({params: {brandCode: 'new'}})}>
                    <div className={brandStyles.expand}>
                      {this.renderAddNewBrandButton()}
                    </div>
                    </PermissionedForNonComponents>
                  </div>
                </div>
              </div>
            </header>
          </div>
          <Collapse className={styles.component}
            defaultActiveKey={this.props.brandCode}
            activeKey={this.state.activePanel}
            onChange={(key, e) => this.setState({ activePanel: key })}>
            {this.renderBrandNewHeader("New Brand")}
            {
              this
                .state
                .brandList
                .map(brand => this.renderHeader(col(brand, 'brand_code', 'value'), brand))
            }

          </Collapse>
          <AlertDialog message={this.state.error.message} title="Warning" shown={this.state.error.shown}
            onClose={this.cancelErrorDialog} />
          <ConfirmationDialog message={`The changes you have to Brand details will not be saved. Do you wish to continue?`} title="Warning"
            shown={this.state.confirmation.shown} onYes={this.yesConfirmationDialog}
            onNo={this.noConfirmationDialog} />
          <AlertDialog message={this.state.error.message} title="Error" shown={this.state.error.shown}
            onClose={this.cancelErrorDialog} />
        </div>
      )
    }
    else
      return <Loading />
  }
}
