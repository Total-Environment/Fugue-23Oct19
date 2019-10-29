import React, {PropTypes} from 'react';
import {Tab, Tabs, TabList, TabPanel} from 'react-tabs';
import styles from './index.css';
import {Icon} from '../icon';
import {containerName, getHeader, getSaasToken, head, idFor} from '../../helpers';
import DataType from '../data-types';
import {Link} from 'react-router';

const additionalColumnInfo = (value, headerKey, headerKeys, isListing) => {
  const columns = value.headers.find(header => header.key === headerKey).columns;
  let columnKeys = columns.map(column => column.key);
  let allColumnKeys = columnKeys.slice(0);
  let systemLogColumns = [];
  if (typeof headerKeys !== "undefined") {
    let systemLogHeaderKey = headerKeys.filter(headerKey => headerKey === 'system_logs');
    let systemLogHeader = value.headers.find(header => header.key === systemLogHeaderKey);
    systemLogColumns = (systemLogHeader && systemLogHeader.columns) || [];
    const systemLogColumnKeys = systemLogColumns && systemLogColumns.filter(column =>
      column.key === 'date_created'
      || column.key === 'created_by').map(column => column.name) || [];
    if (typeof isListing !== "undefined")
      Array.prototype.push.apply(allColumnKeys, systemLogColumnKeys);
  }
  return {
    'allColumnKeys': allColumnKeys,
    'systemLogColumns': systemLogColumns
  }
};

const renderMaterialBasicInfo = (material) => {
  const imageColumn = head(material, 'general', 'image');
  const statusColumn = head(material, 'general', 'material_status');
  const materialCode = head(material, 'general', 'material_code', 'value');
  const materialName = head(material, 'general', 'material_name', 'value');
  return <div className={styles.basicInfo}>
    <div className={styles.imageContainer}>
      {imageColumn.value && imageColumn.value[0] && imageColumn.value[0].url ? <img className={styles.materialImage}
                                                                                    src={imageColumn.value && imageColumn.value[0] && imageColumn.value[0].url+'?'+getSaasToken(containerName)}/> :
        <Icon name="noImage" className="noImage"/>}
      <DataType className={styles.materialStatus} columnName="material Status" columnValue={statusColumn}
                componentType="material"/>
    </div>
    <h4 className={styles.materialName}>{materialName}</h4>
    <h5 className={styles.materialCode}>{materialCode}</h5>
  </div>
};

const renderServiceBasicInfo = (service) => {
  const serviceCode = head(service, 'general', 'service_code', 'value');
  return <div className={styles.basicInfo}>
    <dt id={idFor(name, 'title')} className={styles.columnTitle}>Service Code</dt>
    <dd id={idFor(name, 'value')} className={styles.columnValue}>
      <h4 className={styles.materialCode}>{serviceCode}</h4>
    </dd>
    <DataType className={styles.serviceStatus} columnName="service Status"
              columnValue={head(service, 'general', 'service_status')}/>
  </div>
};

const renderSfgBasicInfo = (sfg) => {
  return <div className={styles.basicInfo}>
    <dt id={idFor(name, 'title')} className={styles.columnTitle}>SFG Code</dt>
    <dd id={idFor(name, 'value')} className={styles.columnValue}>
      <DataType columnName='SFG Code' columnValue={head(sfg, 'general', 'sfg_code')} componentType="SFG"/>
    </dd>
    <DataType className={styles.sfgStatus} columnName="SFG Status"
              columnValue={head(sfg, 'general', 'sfg_status')}/>
  </div>
};

const renderPackageBasicInfo = (pkg) => {
  const imageColumn = head(pkg, 'general', 'image');
  const statusColumn = head(pkg, 'general', 'pkg_status');
  const packageCode = head(pkg, 'general', 'package_code', 'value');
  return <div className={styles.basicInfo}>
    <div className={styles.imageContainer}>
      {imageColumn.value && imageColumn.value[0] && imageColumn.value[0].url ? <img className={styles.materialImage}
                                                                                    src={imageColumn.value && imageColumn.value[0] && imageColumn.value[0].url+'?'+getSaasToken(containerName)}/> :
        <Icon name="noImage" className="noImage"/>}
      <DataType className={styles.materialStatus} columnName="Package Status" columnValue={statusColumn}
                componentType="package"/>
    </div>
    <h5 className={styles.materialName}>{packageCode}</h5>
  </div>
};

const renderColumn = (column, componentType) => {
  return <div key={column.name} className={styles.column}>
    <dt id={idFor(column.name, 'title')} className={styles.columnTitle}>{column.name}</dt>
    <dd id={idFor(column.name, 'value')} className={styles.columnValue}>
      <DataType columnName={column.name} columnValue={column} componentType={componentType}/>
    </dd>
  </div>
};

class SearchComponentDetails extends React.Component {
  getChildContext() {
    return {
      componentType: this.props.componentType,
      componentCode: head(this.props.value, 'general',`${this.props.componentType.toLowerCase()}_code`, 'value')
    };
  }

  render() {
    const {headerKey, value, headerKeys, componentType, isListing} = this.props;
    const columns = getHeader(value, headerKey).columns;
    const additionalInfo = additionalColumnInfo(value, headerKey, headerKeys, isListing);
    let hiddenColumns;
    if (componentType === 'package') {
      hiddenColumns = ['image', `${componentType.toLowerCase()}_status`, `${componentType.toLowerCase()}_code`, 'definition_source',
        'breakdown_allowed', 'pkg_status'];
    }
    else {
      hiddenColumns = ['image', `${componentType.toLowerCase()}_status`, `${componentType.toLowerCase()}_code`];
    }
    const basicInfoRenderers = {
      material: renderMaterialBasicInfo,
      service: renderServiceBasicInfo,
      SFG: renderSfgBasicInfo,
      package: renderPackageBasicInfo
    };
    let code;
    if (componentType === 'sfg')
      code = "SFG";
    else
      code = componentType;
    return <div className={styles.columnsRow}>
      <Link className={styles.link}
            to={`/${componentType}s/${head(value, 'general', `${code.toLowerCase()}_code`, 'value')}`}>
        {basicInfoRenderers[componentType](value)}
      </Link>
      <div className={styles.otherDetails}>
        {additionalInfo.allColumnKeys
          .filter(columnKey => !hiddenColumns.includes(columnKey))
          .map(columnKey => {
            let column = columns.find(column => column.key === columnKey);
            let systemLogColumn = additionalInfo.systemLogColumns.find(column => column.key === columnKey);
              return renderColumn( column || systemLogColumn ,componentType)
            }
          )}
      </div>
    </div>
  }
}
SearchComponentDetails.childContextTypes = {
  componentType: PropTypes.string,
  componentCode: PropTypes.string,
};

export class PageTab extends React.Component {

  constructor(props) {
    super(props);
    this.state = {selectedIndex: 1};
  }

  renderTabBody(values, headerKey, headerKeys) {
    const props = {headerKey, headerKeys, componentType: this.props.componentType, isListing: this.props.isListing};
    return <TabPanel>{values.map(value => <SearchComponentDetails value={value} {...props} />)}</TabPanel>;
  }

  render() {
    const headerNames = this.props.values[0].headers.map(header => header.name);
    const headerKeys = this.props.values[0].headers.map(header => header.key);
    if (typeof this.props.isListing === "undefined") {
      const headerIndex = headerKeys.indexOf(this.props.selectedTab);
      return <Tabs selectedIndex={headerIndex} onSelect={i => this.props.onTabChange(headerKeys[i])}>
        <TabList>
          {headerNames.map(header => <Tab id={idFor(header)}>{header}</Tab>)}
        </TabList>
        {headerKeys.map(headerKey => this.renderTabBody(this.props.values, headerKey))}
      </Tabs>;
    } else {
      return (<Tabs selectedIndex={0}>
        <TabList>
          <Tab>Recently Added</Tab>
          <Tab disabled="true">Updated</Tab>
          <Tab disabled="true">Going out of production</Tab>
        </TabList>
        {headerKeys.filter(h => h.toLowerCase() === "general" || h.toLowerCase() === "system logs")
          .map(headerKey => this.renderTabBody(this.props.values, headerKey, headerKeys))}
      </Tabs>);
    }
  }
}
