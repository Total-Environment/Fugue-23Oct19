import React from 'react';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import * as R from 'ramda';
import { Loading } from '../loading';
import Pagination from 'rc-pagination';
import flexboxgrid from 'flexboxgrid/css/flexboxgrid.css';
import componentListStyles from './component-list.css';
import searchStyles from './search.css';
import classNames from 'classnames';
import DataType from '../data-types';
import {head, idFor} from '../../helpers';
import { Icon } from '../icon/index';
import { button } from '../../css-common/forms.css';
import locale from 'rc-pagination/lib/locale/en_US';

const renderMaterialBasicInfo = (material) => {
  const imageColumn = head(material,'general','image');
  const statusColumn = head(material,'general','material_status');
  const materialCode = head(material,'general','material_code','value');
  const materialName = head(material,'general','material_name','value');
  return <div className={componentListStyles.basicInfo}>
    <div className={componentListStyles.imageContainer}>
      <img className={componentListStyles.materialImage}
        src={imageColumn.value && imageColumn.value[0] && imageColumn.value[0].url} />
      <DataType className={componentListStyles.materialStatus} columnName={statusColumn.name} columnValue={statusColumn}
        componentType="material" />
    </div>
    <h4 className={componentListStyles.materialName}>{materialName}</h4>
    <h5 className={classNames(componentListStyles.materialCode)}>{materialCode}</h5>
  </div>
};

const renderServiceBasicInfo = (service) => {
  const serviceCode = head(service,'general','service_code');
  const serviceStatus = head(service,'general','service_status');
  return <div className={componentListStyles.basicInfo}>
    <dt id={idFor(name, 'title')} className={componentListStyles.columnTitle}>Service Code</dt>
    <dd id={idFor(name, 'value')} className={componentListStyles.columnValue}>
      <DataType columnName={serviceCode.name} columnValue={serviceCode} componentType="service" />
    </dd>
    <DataType className={componentListStyles.serviceStatus} columnName={serviceStatus.name}
      columnValue={serviceStatus} />
  </div>
};

const renderSFGInfo = (sfg) => {
  const sfgCode = head(sfg,'general','sfg_code');
  const sfgStatus = head(sfg,'general','sfg_status');
  return <div className={componentListStyles.basicInfo}>
    <dt id={idFor(name, 'title')} className={componentListStyles.columnTitle}>SFG Code</dt>
    <dd id={idFor(name, 'value')} className={componentListStyles.columnValue}>
      <DataType columnName={sfgCode.name} columnValue={sfgCode} componentType="service" />
    </dd>
    <DataType className={componentListStyles.serviceStatus} columnName={sfgStatus.name}
      columnValue={sfgStatus} />
  </div>
};

export class ComponentList extends React.Component {
  constructor(props) {
    super(props);
    this._renderBasicInfo = this._renderBasicInfo.bind(this);
    this._renderColumn = this._renderColumn.bind(this);
    this._onSelectItem = this._onSelectItem.bind(this);
    this._onChooseClick = this._onChooseClick.bind(this);
    this._getActiveComponent = this._getActiveComponent.bind(this);
    this.initialState = {
      selectedComponentCodes: [],
      selectedComponents: [],
      currentTabIndex: 0
    };

    this.state = R.clone(this.initialState);
  }
  _getActiveComponent() {
    if (this.props.activeResource.toLowerCase() === 'sfg') {
      return 'sfg';
    }
    return this.props.activeResource === 'asset' ? 'material' : this.props.activeResource;
  }
  _onChooseClick(e) {
    e.preventDefault();
    let chosenComponents = this.state.selectedComponents || [];
    this.props.onChooseComponents(chosenComponents);
    this.setState(this.initialState);
  }
  _onSelectItem(type, code, details) {
    let currentSelectedComponentCodes = this.state.selectedComponentCodes;
    let currentSelectedComponents = this.state.selectedComponents;


    if (R.indexOf(code, currentSelectedComponentCodes) > -1) {
      return this.setState({
        selectedComponentCodes: currentSelectedComponentCodes.filter(c => c !== code),
        selectedComponents: currentSelectedComponents.filter(c => c.id !== code)
      })
    }
    return this.setState({
      selectedComponentCodes: R.append(code, currentSelectedComponentCodes),
      selectedComponents: R.append(R.merge(details, { type }), currentSelectedComponents)
    });
  }
  _renderColumn(column) {
    return <div key={column.name} className={componentListStyles.column}>
      <dt id={idFor(column.name, 'title')} className={componentListStyles.columnTitle}>{column.name}</dt>
      <dd id={idFor(column.name, 'value')} className={componentListStyles.columnValue}>
        <DataType columnName={column.key} columnValue={column} componentType={this.props.activeResource} />
      </dd>
    </div>;
  }
  _renderDetailsRow(details) {
    let componentType = this._getActiveComponent();
    const hiddenColumns = ['image', `${componentType}_status`, `${componentType}_code`];
    if (!details) {
      return <div></div>;
    }
    let columns = details.columns
      .filter(c => !hiddenColumns.includes(c.key));
    return columns.map(column => this._renderColumn(column));
  }
  _renderBasicInfo(details) {
    if (this._getActiveComponent().toLowerCase() === 'sfg') {
      return renderSFGInfo(details);
    }
    return this._getActiveComponent() === 'material' ? renderMaterialBasicInfo(details) : renderServiceBasicInfo(details);
  }
  _renderColumnListing(result, columnKey) {
    let values = (result && result.items) || [];
    return (values.map(
      (item, i) => {
        let generalDetails = item.headers.find(header => header.key ==='general') || null;
        let specificationDetails = item.headers.find(header => header.key === 'specifications') || null;
        let componentCode = (generalDetails && generalDetails.columns.find(column => column.key === `${this._getActiveComponent()}_code`)) || '';
        let isSelected = this.state.selectedComponentCodes.indexOf(componentCode.value) > -1;
        return <div className={
          classNames(componentListStyles.columnsRow)}
          onClick={e => {
            e.preventDefault();
            return this._onSelectItem(this.props.activeResource, componentCode.value, item)
          }}
          id={`${columnKey}_${componentCode.value}`}
          key={`${columnKey}_${componentCode.value}`}>
          <div className={componentListStyles.link}>
            {this._renderBasicInfo(item)}
          </div>
          <div className={componentListStyles.otherDetails}>
            {this._renderDetailsRow(columnKey === 'general' ? generalDetails : specificationDetails)}
          </div>
          <div className={componentListStyles.selected}>
            {isSelected ? <input type="checkbox"
              name={`checked-${componentCode.value}`}
              checked={true}
            /> : <span id={idFor("check box")} className={componentListStyles.uncheckedCheckbox} />}</div>
        </div>;
      })
    );
  }
  componentWillReceiveProps(nextProps) {
    let componentType = nextProps.data && nextProps.data.componentType || null;
    if (nextProps.activeResource !== componentType) {
      return this.setState({
        currentTabIndex: 0
      });
    }
  }
  render() {
    let componentType = this.props.data && this.props.data.componentType || null;
    if (R.isNil(this.props.data)
      || !this.props.activeResource
      || !componentType) {
      return <div></div>;
    }
    let result = this.props.data && this.props.data.values || null;
    let isFetching = (this.props.data && this.props.data.isFetching) || false;

    if (isFetching) {
      return <div className={componentListStyles.loading}>
        <Loading />
      </div>
    }
    if (this.props.activeResource !== componentType) {
      return <div></div>;
    }
    if (!isFetching && R.isNil(result) && !R.isNil(this.props.data) && this.props.activeResource) {
      return (
        <h2>No {this.props.activeResource}s found</h2>
      )
    }
    return (
      <div className={componentListStyles.tabContainer}>
        <Tabs selectedIndex={this.state.currentTabIndex} onSelect={index => this.setState({ currentTabIndex: index })}>
          <TabList>
            {this.props.headers
              .filter(c => {
                if (componentType === 'service' || componentType === 'sfg') {
                  return c.key !== 'specifications';
                }
                return true;
              })
              .map(c => <Tab id={c.key} key={c.key}>{c.name}</Tab>)}
          </TabList>
          {this.props.headers
            .filter(c => {
              if (componentType === 'service' || componentType === 'sfg') {
                return c.key !== 'specifications';
              }
              return true;
            })
            .map(c => <TabPanel
              key={`${c.key}_panel`}
              className={componentListStyles.panel}>
              {this._renderColumnListing(result, c.key)}
            </TabPanel>)}
        </Tabs>
        <Pagination
          total={result.recordCount}
          pageSize={result.batchSize > -1 ? result.batchSize : 5}
          onChange={pageNumber => this.props.onTriggerSearch(pageNumber)}
          current={result.pageNumber}
          locale={locale}
        />
        <input id="choose-resources"
          className={classNames(button, componentListStyles.chooseButton)}
          type="button"
          onClick={this._onChooseClick}
          value="Add" />
      </div>
    );
  }
};
