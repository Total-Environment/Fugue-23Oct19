import React from 'react';
import Collapse from 'rc-collapse';
import styles from './index.css';
import listStyles from './component-list.css';
import {SearchComponent} from './search-component';
import {Icon} from '../icon';
import {CompositeComposition} from './composition';
import * as R from 'ramda';
import {isFetched} from "../../helpers";
import {transformCompositionDetail, transformCompositionDetailToColumns} from './helper';
import {CreateHeaderColumn} from "../create-header-column";
import {button} from '../../css-common/forms.css';
import classNames from 'classnames';
import {alertAsync, confirmAsync} from '../dialog';
import {validate} from '../../validator';
import {browserHistory} from 'react-router';
import {col, head} from '../../helpers';
import {Loading} from '../loading/index';
import flexboxgrid from 'flexboxgrid/css/flexboxgrid.css';
import Modal from 'react-modal';

const componentTypes = {'sfg':'SFG', 'package': 'Package'};

export class CompositeDetails extends React.Component {
  constructor(props) {
    super(props);
    // state
    this.initialState = {
      compositionDetails: [],
      activeKey: 'sfg-composition',
      sfgDetails: null,
      isComponentSearchOpen: false
    };

    this.state = R.clone(this.initialState);
    // Bindings.
    this._fetchResourceTypes = this._fetchResourceTypes.bind(this);
    this._renderCollapseContainer = this._renderCollapseContainer.bind(this);
    this._renderComponentSearchContainer = this._renderComponentSearchContainer.bind(this);
    this.fetchActiveCollapseKeys = this.fetchActiveCollapseKeys.bind(this);
    this._onTriggerSearch = this._onTriggerSearch.bind(this);
    this._onChooseComponents = this._onChooseComponents.bind(this);
    this._onRemoveCompositionItem = this._onRemoveCompositionItem.bind(this);
    this._onMakePrimaryItem = this._onMakePrimaryItem.bind(this);
    this._hasPrimaryResource = this._hasPrimaryResource.bind(this);
    this._onCompositionDetailChange = this._onCompositionDetailChange.bind(this);
    this._validateColumn = this._validateColumn.bind(this);
    this._validateSFGCompositionData = this._validateSFGCompositionData.bind(this);
    this._validateData = this._validateData.bind(this);
    this._handleSFGDetailsChange = this._handleSFGDetailsChange.bind(this);
    this._validateSFGDetails = this._validateSFGDetails.bind(this);
    this._onCancelClick = this._onCancelClick.bind(this);
    this._handleSubmit = this._handleSubmit.bind(this);
    this._constructDataToSave = this._constructDataToSave.bind(this);
    this._getPrimaryResource = this._getPrimaryResource.bind(this);
    this._isValidDependency = this._isValidDependency.bind(this);
    this._matchesDefinition = this._matchesDefinition.bind(this);
    this._showComponentSearch = this._showComponentSearch.bind(this);
    this.handleAddComposition = this.handleAddComposition.bind(this);
  }

  componentWillUnmount() {
    this.props.onSearchDestroy();
  }

  componentWillMount() {
    this.props.fetchDependencyDefinition('material');
    this.props.fetchDependencyDefinition('service');
    this.props.fetchDependencyDefinition('sfg');
    this.props.fetchDependencyDefinition('package');
    if (!isFetched(this.props.statusData)) {
      this.props.fetchMasterDataByName('status');
    }
    this.props.mode === 'edit' && this.props.componentType === 'sfg' && this.props.onSfgFetchRequest(this.props.compositeCode);
    this.props.mode === 'edit' && this.props.componentType === 'package' && this.props.onPackageFetchRequest(this.props.compositeCode);
    if (!isFetched(this.props.definition)) {
      this.props.componentType === 'package' ? this.props.fetchPackageDefinition() : this.props.fetchSfgDefinition();
    }
  }

  componentWillReceiveProps(nextProps) {
    let err = nextProps.classificationDataError[nextProps.componentType] !== '' || nextProps.masterDataError || nextProps.sfgError;
    if (err) {
      return;
    }
    if (isFetched(this.props.sfgData) && isFetched(this.props.definition) && R.isNil(this.state.sfgDetails) && this.props.mode === 'edit') {
      let values = this.props.sfgData.values;
      let compositionDetails = (values && values.componentComposition && values.componentComposition.componentCoefficients) || [];
      let updatedSfgDetails = R.merge(this.props.definition.values, {
        headers: this.props.definition.values.headers.map(header => R.merge(header, {
          columns: header.columns.map(column => R.merge(column, {
            editable: !R.contains('level_1', column.key) && (!(column.key === "weighted_average_purchase_rate" || column.key === "last_purchase_rate")),
            validity: {isValid: true, msg: ''},
            value: head(values, header.key, column.key, 'value') || ''
          }))
        }))
      });
      return this.setState({
        sfgDetails: updatedSfgDetails,
        compositionDetails: (compositionDetails && compositionDetails.map(c => ({
          id: c.code,
          values: transformCompositionDetailToColumns(c)
        }))) || []
      });
    }
    if (isFetched(this.props.definition) && R.isNil(this.state.sfgDetails) && this.props.mode === 'create') {
      return this.setState({
        sfgDetails: R.merge(this.props.definition.values, {
          headers: this.props.definition.values.headers.map(header => R.merge(header,
            {
              columns: header.columns.map(column => R.merge(column,
                {editable: (!(column.key === "weighted_average_purchase_rate" || column.key === "last_purchase_rate")), validity: {isValid: true, msg: ''}})
              )
            }))
        })
      });
    }
  }

  _matchesDefinition(children, values) {
    if (values.length === 0) {
      return children.length === 0;
    }
    const currentValue = values[0] || "null";
    const currentColumn = children.find(column => column.name === currentValue);
    if (!currentColumn) return false;
    return this._matchesDefinition(currentColumn.children, values.slice(1));
  }

  _isValidDependency() {
    const children = this.props.classificationData[`${this.props.componentType.toLowerCase()}Classifications`].values.block.children;
    const classificationValues = this.state.sfgDetails.headers.find(c => c.key === 'classification').columns.map(c => c.value || "");
    return this._matchesDefinition(children, classificationValues);
  }

  _constructDataToSave() {
    let keys = {
      'resource_type': 'componentType',
      'unit_of_measure': 'unitOfMeasure'
    };
    let ignoreKeys = ['component_code', 'component_name'];
    let componentCoefficients = this.state.compositionDetails.map(h => {
      return R.merge({
        code: h.id
      }, R.reduce((acc, ele) => {
        if (R.contains(ele.key, ignoreKeys)) return acc;
        return R.contains('wastage', ele.key) ? R.merge(acc, {
          wastagePercentages: ele.value && +(ele.value) > 0 ? R.append({
            name: ele.name,
            value: ele.value
          }, acc.wastagePercentages || []) : (acc.wastagePercentages || [])
        }) : R.merge(acc, {
          [keys[ele.key] || ele.key]: ele.value
        });
      }, {}, h.values));
    });

    let classificationHeader = this.state.sfgDetails.headers.find(h => h.key === 'classification');
    let group = (classificationHeader.columns.find(c => R.contains('level_1', c.key)) || {}).value || '';
    let sfgDetails = this._getPrimaryResource() ? {} : {
      group,
      headers: this.state.sfgDetails.headers.map(h => {
        return {
          columns: h.columns.map(c => ({
            key: c.key,
            name: c.name,
            value: R.isEmpty(c.value) && R.is(String, c.value) ? null : c.value,
            dataType: c.dataType
          })),
          key: h.key,
          name: h.name
        }
      })
    };

    return R.merge(sfgDetails, {
      componentComposition: {
        componentCoefficients
      }
    });
  }

  _handleSubmit(e) {
    e.preventDefault();
    if (!this._hasPrimaryResource() && !this._isValidDependency()) {
      return alertAsync('Error',
        <span>Classification values submitted for the {this.props.componentType} are not valid (or) are incomplete. <br/>Please define a valid combination or reach out to Admin for further help.</span>);
    }
    if (!this._validateData()) {
      return false;
    }
    let data = this._constructDataToSave();
    let generalHeader = data.headers && data.headers.find(c => c.key === 'general');
    let compositeCode = generalHeader && generalHeader.columns && generalHeader.columns.find(c => c.key === `${this.props.componentType.toLowerCase()}_code`);
    return this.props.saveCompositeDetails(
      this._hasPrimaryResource() ? data.componentComposition : data,
      this.props.componentType.toLowerCase(),
      this._hasPrimaryResource() ? this._getPrimaryResource().id : null,
      this.props.mode === 'edit' ? (compositeCode && compositeCode.value) : null
    );
  }

  _onCancelClick() {
    let data = this._constructDataToSave();
    let generalHeader = data.headers && data.headers.find(c => c.key === 'general');
    let compositeCode = generalHeader && generalHeader.columns && generalHeader.columns.find(c => c.key === `${this.props.componentType.toLowerCase()}_code`);
    let path = this.props.mode === 'edit' ? `/${this.props.componentType.toLowerCase()}s/${this.props.compositeCode}` : `/${this.props.componentType.toLowerCase()}s`;
    return confirmAsync('Warning', this.props.mode === 'edit' ? `The changes made to ${componentTypes[this.props.componentType]} ${compositeCode.value} will not be saved. Do you wish to continue?` : `The ${componentTypes[this.props.componentType]} will not be created. Do you wish to continue?`).then(() => {
      return browserHistory.push(path);
    });
  }

  _validateData() {
    if (this.state.compositionDetails.length < 2 && this.props.componentType === 'sfg') {
      return alertAsync('Error', 'Minimum of two components should be associated to create an SFG. Please select another Material/Service/Asset') && false;
    }
    if (this.state.compositionDetails.length < 1 && this.props.componentType === 'package') {
      return alertAsync('Error', 'Minimum of one component should be associated to create a Package. Please select another Material/Service/Asset/SFG') && false;
    }
    if (!this._validateSFGCompositionData()) {
      return alertAsync("Error", <span>The {componentTypes[this.props.componentType]} details submitted are either incomplete (or) not valid. <br/>Please check and fix the errors in the form and resubmit.</span>) && false;
    }
    if (!this._hasPrimaryResource() && !this._validateSFGDetails()) {
      return alertAsync("Error", <span>The {componentTypes[this.props.componentType]} details submitted are either incomplete (or) not valid. <br/>Please check and fix the errors in the form and resubmit.</span>) && false;
    }
    return true;
  }

  _validateSFGDetails() {
    let validatedSFGDetailsHeaders = ((this.state.sfgDetails && this.state.sfgDetails.headers) || []).map(h => Object.assign({}, h, {
      columns: h.columns.map(c => R.merge(c, {
        validity: this._validateColumn(c.value, c, c.name)
      }))
    }));
    this.setState({
      sfgDetails: R.merge(this.state.sfgDetails, {
        headers: validatedSFGDetailsHeaders
      })
    });
    return validatedSFGDetailsHeaders.reduce((acc, h) => h.columns.reduce((accT, c) => (c.validity.isValid || false) && accT, acc), true);
  }

  _validateSFGCompositionData() {
    let defaultValue = key => R.contains(key, ['coefficient', 'wastage']) ? "0" : "";
    let validatedCompositionDetails = this.state.compositionDetails.map(c => Object.assign({}, c, {
      values: c.values.map(d => Object.assign({}, d, {validity: this._validateColumn(d.value || defaultValue(d.key), d, d.name)}))
    }));
    this.setState({
      compositionDetails: validatedCompositionDetails
    });
    return validatedCompositionDetails.reduce((acc, c) => c.values.reduce((accT, d) => !!(d.validity && d.validity.isValid) && accT, true) && acc, true);
  }

  _validateColumn(value, column, name) {
    return validate(value, column, name, column.key === 'coefficient' ? {
      validate: value => {
        let isValid = +(value) > 0;
        return {isValid, msg: isValid ? '' : 'Coefficient must be greater than 0'};
      }
    } : null);
  }

  _handleSFGDetailsChange(headerKey, columnKey, value,columnName) {
    if(this.props.mode === "edit") {
      if (this.props.componentType === "sfg" && (columnKey === "sfg_status" || columnKey === "unit_of_measure")) {
        alertAsync('Warning', `Changing ${columnName} will impact existing purchase orders in Fugue. Existing purchase orders need to be closed before the ${columnName} is updated.`);
      }
      if (this.props.componentType === "package" && (columnKey === "pkg_status" || columnKey === "unit_of_measure")) {
        alertAsync('Warning', `Changing ${columnName} will impact existing service orders in Fugue. Existing service orders need to be closed before the ${columnName} is updated.`);
      }
    }
    return this.setState({
      sfgDetails: R.merge(this.state.sfgDetails, {
        headers: this.state.sfgDetails.headers.map(h => h.key === headerKey ? R.merge(h, {
          columns: h.columns.map(c => c.key === columnKey ? R.merge(c, {
            value,
            validity: this._validateColumn(value, c, c.name)
          }) : c)
        }) : h)
      })
    });
  }

  _onCompositionDetailChange(code, column, value) {
    let key = column.key;
    let updatedCompositionDetails = this.state.compositionDetails.map(c => {
      if (c.id === code) {
        return Object.assign({}, c, {
          values: c.values.map(d => {
            if (d.key === key) {
              return Object.assign({}, d, {
                value,
                validity: this._validateColumn(value, column, column.name)
              });
            }
            return d;
          })
        });
      }
      return c;
    });

    this.setState({
      compositionDetails: updatedCompositionDetails
    });
  }

  _hasPrimaryResource() {
    if (this.state.compositionDetails.length <= 0) {
      return true;
    }
    return this.state.compositionDetails.some(c => c.isPrimary);
  }

  _getPrimaryResource() {
    return this.state.compositionDetails.find(c => c.isPrimary) || null;
  }

  _onMakePrimaryItem(compCode) {
    this.setState({
      compositionDetails: this.state.compositionDetails
        .map(c => Object.assign({}, c, c.id === compCode ? {isPrimary: !c.isPrimary} : {isPrimary: false}))
    });
  }

  _onRemoveCompositionItem(compCode) {
    let updatedCompositionDetails = R.filter(c => c.id !== compCode, this.state.compositionDetails)
    this.setState({
      compositionDetails: R.filter(c => c.id !== compCode, this.state.compositionDetails)
    });
  }

  _onChooseComponents(components) {
    let updatedCompositionDetails = R.uniqBy(c => c.id,
      R.concat(this.state.compositionDetails, components.map(c => ({
        id: c.id || c.code,
        values: transformCompositionDetail(c)
      }))));
    let updatedState = {
      compositionDetails: updatedCompositionDetails,
      isComponentSearchOpen: false
    };
    if (!R.isEmpty(updatedCompositionDetails)) {
      updatedState.activeKey = 'sfg-composition';
    }
    this.setState(updatedState);
  }

  _onTriggerSearch(componentType, pageNumber, group, filterData) {
    this.props.searchComponents(componentType, pageNumber, group, filterData);
  }

  _fetchResourceTypes() {
    let defaultList = ['Material', 'Service', 'Asset'];
    return this.props.resourceTypes || (this.props.componentType === 'package' ? R.append('SFG', defaultList) : defaultList);
  }

  _showComponentSearch() {
    return isFetched(this.props.statusData) && (
        <Modal id={'composite-comp-search'} className={styles.dialog} isOpen={this.state.isComponentSearchOpen}
               contentLabel="Component Search">
          <div className={styles.searchHeader} id={'composite-comp-search-header'}>
            <h1 className={styles.searchTitle}>Component Search</h1>
            <button className={styles.searchCloseBtn} id={'search-close'} onClick={(e) => {
              e.preventDefault();
              return this.setState({
                isComponentSearchOpen: false
              });
            }}><Icon name="close" className={close}/></button>
          </div>
          <SearchComponent
            componentType={this.props.componentType}
            onTriggerSearch={this._onTriggerSearch}
            resources={
              this._fetchResourceTypes()
            }
            onChooseComponents={this._onChooseComponents}
            {...this.props}
          />
        </Modal>);
  }

  _renderComponentSearchContainer() {
    return isFetched(this.props.statusData) &&
      <Collapse.Panel
        key={SearchComponent.headerKey}
        showArrow={false}
        header={<span id={SearchComponent.headerKey}>
          <Icon name="arrow" className="arrow"/>
          {SearchComponent.headerName}
        </span>}>
        <SearchComponent
          componentType={this.props.componentType}
          onTriggerSearch={this._onTriggerSearch}
          resources={
            this._fetchResourceTypes()
          }
          onChooseComponents={this._onChooseComponents}
          {...this.props}
        />
      </Collapse.Panel>;
  }

  handleAddComposition(e) {
    e.preventDefault();
    e.stopPropagation();
    return this.setState({isComponentSearchOpen: true});
  }

  _renderCompositionContainer() {
    return (
      <Collapse.Panel
        key={'sfg-composition'}
        showArrow={false}
        header={<span id={'sfg-composition'}>
          <Icon name="arrow" className="arrow"/>
          {`${this.props.componentType} Composition`}
        </span>}>
        {this.state.compositionDetails.length > 0 ? <div><CompositeComposition
          onMakePrimaryItem={this._onMakePrimaryItem}
          mode={this.props.mode}
          hasPrimary={true}
          componentType={this.props.componentType}
          onDetailChange={this._onCompositionDetailChange}
          compositionDetails={this.state.compositionDetails}
          onRemoveCompositionItem={this._onRemoveCompositionItem}
        />
          {this.props.mode === 'create' &&
          <p className={styles.addAnotherComposition}><a href="#" onClick={this.handleAddComposition}>Add more</a></p>}
        </div> :
          <div className={styles.center}><p>No
            materials/services/assets{this.props.componentType === 'package' && '/SFGs'} added.</p><p><a href="#"
                                                                                                         onClick={this.handleAddComposition}>Add</a>
          </p></div>}
      </Collapse.Panel>
    )
  }

  _renderDetails() {
    return isFetched(this.props.definition) && isFetched(this.props.classificationData[`${this.props.componentType.toLowerCase()}Classifications`]) &&
      <CreateHeaderColumn definition={this.state.sfgDetails}
                          componentType={this.props.componentType}
                          dependencyDefinition={this.props.classificationData[`${this.props.componentType.toLowerCase()}Classifications`]}
                          masterData={this.props.masterData}
                          onDetailsChange={this._handleSFGDetailsChange}
                          onMasterDataFetch={this.props.onMasterDataFetch}/>;
  }

  fetchActiveCollapseKeys() {
    return [SearchComponent.headerKey, 'sfg-composition'];
  }

  _renderCollapseContainer() {
    return <Collapse
      className={styles.component}
      defaultActiveKey={'sfg-composition'}
      accordion={true}
      activeKey={this.state.activeKey}
      onChange={key => {
        return this.setState({
          activeKey: key
        });
      }}>
      {this.props.mode === 'create' && this._showComponentSearch()}
      {this._renderCompositionContainer()}
    </Collapse>;
  }

  _renderAddCancelButtons() {
    return <div className={styles.actionButtons}>
      <input className={button}
             disabled={this.props.isSaving}
             type="submit"
             value={this.props.isSaving ? 'Saving...' : this.props.mode === 'edit' ? 'Update' : 'Add'}/>
      <a className={button}
         disabled={this.props.isSaving}
         onClick={e => this._onCancelClick()}>Cancel</a>
    </div>
  }

  render() {
    let title = `Create New ${componentTypes[this.props.componentType]}`;
    if (this.props.mode === 'edit' && !isFetched(this.props.definition) && !isFetched(this.props.sfgData)) {
      return <div><Loading /></div>;
    }
    return (
      <div>
        <h1 className={styles.title}>{this.props.mode === 'edit' ? `${this.props.compositeCode}` : title}</h1>
        <form onSubmit={this._handleSubmit}>
          {this._renderCollapseContainer()}
          {!this._hasPrimaryResource() ? this._renderDetails() : ''}
          {this._renderAddCancelButtons()}
        </form>
      </div>
    )
  }
}
