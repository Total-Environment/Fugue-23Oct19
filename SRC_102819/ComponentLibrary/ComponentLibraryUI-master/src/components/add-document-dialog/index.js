import React from 'react';
import Modal from 'react-modal';
import styles from './index.css';
import classNames from 'classnames';
import { Loading } from '../loading';
import { Icon } from "../icon/index";
import { idFor } from '../../helpers';
import Pagination from 'rc-pagination';
import locale from 'rc-pagination/lib/locale/en_US';
import { alertAsync } from '../dialog';

export class AddDocumentDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      document: null,
      keyword: '',
      linkFile: { componentCode: null, document: null },
      invalidMessage: ''
    };
    this.onSelect = this.onSelect.bind(this);
    this.onInvalidSelect = this.onInvalidSelect.bind(this);
    this.onSearch = this.onSearch.bind(this);
    this.onLink = this.onLink.bind(this);
    this._checkForGroup = this._checkForGroup.bind(this);
    this.onPageNumberChange = this.onPageNumberChange.bind(this);
  }

  _checkForGroup() {
    if (!this.props.group) {
      let message = ((compType) => {
        let groupName = '';
        switch (compType && compType.toLowerCase()) {
          case 'sfg':
            groupName = 'SFG Level 1';
            break;
          case 'material':
            groupName = 'Material Level 2';
            break;
          case 'service':
            groupName = 'Service Level 1';
            break;
          case 'package':
            groupName = 'Package Level 1';
            break;
          default:
            break;
        }
        return `Missing "${groupName}"`;
      });
      return alertAsync('Error', message(this.props.componentType) || "Missing Component Group") && false;
    }
    return true;
  }

  getDocumentsByGroupAndColumnName(pageNumber) {
    if (!this._checkForGroup()) {
      return this.props.onCancel();
    }
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.getMaterialDocumentsByGroupAndColumnName(this.props.group, this.props.columnName, pageNumber);
        break;
      case 'service':
        this.props.getServiceDocumentsByGroupAndColumnName(this.props.group, this.props.columnName, pageNumber);
        break;
      case 'brand':
        this.props.getBrandDocumentsByGroupAndColumnName(this.props.group, this.props.columnName, pageNumber);
        break;
      case 'sfg':
        this.props.getSFGDocumentsByGroupAndColumnName(this.props.group, this.props.columnName, pageNumber);
        break;
      case 'package':
        this.props.getPackageDocumentsByGroupAndColumnName(this.props.group, this.props.columnName, pageNumber);
        break;
    }
  }

  getDocumentsByGroupAndColumnNameAndKeyWord(keyword, pageNumber) {
    if (!this._checkForGroup()) {
      return this.props.onCancel();
    }
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.getMaterialDocumentsByGroupAndColumnNameAndKeyWord(
          this.props.group, this.props.columnName, keyword, pageNumber);
        break;
      case 'service':
        this.props.getServiceDocumentsByGroupAndColumnNameAndKeyWord(
          this.props.group, this.props.columnName, keyword, pageNumber);
        break;
      case 'brand':
        this.props.getBrandDocumentsByGroupAndColumnNameAndKeyWord(
          this.props.group, this.props.columnName, keyword, pageNumber);
        break;
      case 'sfg':
        this.props.getSFGDocumentsByGroupAndColumnName(
          this.props.group, this.props.columnName, pageNumber, keyword);
        break;
      case 'package':
        this.props.getPackageDocumentsByGroupAndColumnName(
          this.props.group, this.props.columnName, pageNumber, keyword);
        break;
    }
  }

  componentDidMount() {
    this.getDocumentsByGroupAndColumnName(1);
  }

  componentWillUnmount() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        this.props.destroyMaterialDocuments();
        break;
      case 'service':
        this.props.destroyServiceDocuments();
        break;
      case 'brand':
        this.props.destroyBrandDocuments();
        break;
      case 'sfg':
        this.props.destroySFGDocuments();
        break;
      case 'package':
        this.props.destroyPackageDocuments();
        break;
    }
  }

  renderDocumentName() {
    if (this.state.document !== null) {
      return (<div className={styles.row}>
        <div className={styles.fileName}>
          {typeof (this.state.document) === 'number' ? window.fileList[this.state.document][0].name : this.state.document.name}
        </div>
        <button className={styles.delete} onClick={(e) => {
          e.preventDefault();
          this.setState({ document: null, linkFile: { componentCode: null, id: null } });
        }}>×
        </button>
      </div>);
    }
  }

  renderInvalidMessage() {
    if (this.state.invalidMessage !== null) {
      return (<div className={styles.row}>
        <div className={styles.invalidMessage}>
          {this.state.invalidMessage}
        </div>
      </div>);
    }
  }

  onSelect(e) {
    this.setState({ document: e.indexOfFiles, linkFile: { componentCode: null, document: null }, invalidMessage: null });
  }

  onLink(e) {
    this.setState({ document: e.document, linkFile: { componentCode: e.componentCode, document: e.document }, invalidMessage: null });
  }

  onInvalidSelect(e) {
    this.setState({ document: null, linkFile: { componentCode: null, document: null }, invalidMessage: e.msg });
  }

  onPageNumberChange(e) {
    if (this.state.keyword == '') {
      this.getDocumentsByGroupAndColumnName(e.pageNumber);
    } else {
      this.getDocumentsByGroupAndColumnNameAndKeyWord(this.state.keyword, e.pageNumber);
    }
  }

  onSearch(e) {
    this.setState({ keyword: e.keyword });
    if (e.keyword !== '')
      this.getDocumentsByGroupAndColumnNameAndKeyWord(e.keyword, 1);
    else
      this.getDocumentsByGroupAndColumnName(1);
  }

  render() {
    return (
      <Modal id={idFor('AddDocumentDialogModal')} className={styles.dialog} isOpen={true} contentLabel="Add file">
        <div className={styles.popup}>
          <h3>{this.props.group}</h3>
          {this.renderDocumentName()}
          {this.renderInvalidMessage()}
          {this.props.showLocalSystem &&
            <SelectFromLocalSystem columnName={this.props.columnName} onSelect={this.onSelect} onInvalidSelect={this.onInvalidSelect} />
          }
          <SearchInFileDirectory componentType={this.props.componentType} onSearch={this.onSearch} />
          <ComponentDocuments componentType={this.props.componentType} columnName={this.props.columnName}
            columnHeader={this.props.columnHeader}
            data={this.props.data} isFetching={this.props.isFetching} linkFile={this.state.linkFile}
            error={this.props.error} onLink={this.onLink}
            onPageNumberChange={this.onPageNumberChange} />
          <div className={styles.action}>
            <button id={idFor('Cancel')} className={styles.cancel} onClick={async (e) => {
              e.preventDefault();
              this.props.onCancel();
            }}>Cancel
            </button>
            <button id={idFor('Ok')} className={styles.ok} onClick={async (e) => {
              e.preventDefault();
              this.props.onOk({ document: this.state.document });
            }}>OK
            </button>
          </div>
        </div>
      </Modal>
    );
  }
}

export class SelectFromLocalSystem extends React.Component {
  constructor(props) {
    super(props);
    this.onFileChange = this.onFileChange.bind(this);
  }

  onFileChange(e) {
    let validity;
    if (this.props.columnName.toLowerCase() === "image") {
      const isImage = (/\.(jpg|jpeg|png|PNG|JPG|JPEG)$/i).test(e.target.files[0].name);
      validity = isImage ? { isValid: true, msg: '' } : {
        isValid: false,
        msg: 'Invalid file format. \nPlease upload the following file formats only: jpg, jpeg, png, PNG, JPG, JPEG'
      };
    } else {
      const isStaticFile = (/\.(jpg|JPG|jpeg|png|pdf|PNG|PDF|JPEG)$/i).test(e.target.files[0].name);
      validity = isStaticFile ? { isValid: true, msg: '' } : {
        isValid: false,
        msg: 'Invalid file format. \nPlease upload the following file formats only: jpg, jpeg, png, PNG, JPG, pdf, PDF, JPEG'
      };
    }
    if (validity.isValid === true) {
      window.fileList.push(e.target.files);
      this.props.onSelect({ indexOfFiles: window.fileList.indexOf(e.target.files) })
    } else {
      this.props.onInvalidSelect({ msg: validity.msg });
    }
  }

  render() {
    if (!window.fileList) {
      window.fileList = [];
    }
    const classes = classNames(styles.file);
    return (
      <div className={styles.localSystem}>
        <span><b>Select file from your local system</b></span>
        <div className={styles.fileInput}>
          <input type="file" className={classes} onChange={this.onFileChange} />
          <button className={styles.fakeFile} onClick={(e) => {
            e.preventDefault();
          }}>Browse
          </button>
        </div>
      </div>
    );
  }
}

export class SearchInFileDirectory extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      keywordToSearch: ''
    }
  }

  validKeywords() {
    return this.state.keywordToSearch.split(' ')
      .filter(w => w.length >= 3)
      .reduce((s, b) => s + ' ' + b, '');
  }

  renderNormalInput() {
    let placeHolder;
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        placeHolder = "Search with material code or short description";
        break;
      case 'service':
        placeHolder = "Search with service code or short description";
        break;
      case 'brand':
        placeHolder = "Search with brand code / manufacturer's name / material code";
        break;
      case 'sfg':
        placeHolder = "Search with sfg code / short description";
        break;
    }
    return [
      <input type="text" className={styles.searchInput}
        key="input"
        id={idFor('keyword to search')}
        value={this.state.keywordToSearch}
        placeholder={placeHolder}
        onChange={(e) => {
          this.setState({ keywordToSearch: e.target.value })
        }}
      />,
      <button className={styles.iconContainer}
        key="button"
        id={idFor('search button')}
        onClick={(e) => {
          e.preventDefault();
          let keyword = this.validKeywords();
          if (keyword.length <= 2) {
            return;
          }
          this.props.onSearch({ keyword });
        }}
      >
        <Icon name="search" className={classNames({
          [styles.icon]: true,
          [this.props.className]: true
        })} />
      </button>,
      this.state.keywordToSearch && this.state.keywordToSearch.length <= 2 ? <div className={styles.searchKeywordErr}>Search Keyword must be atleast of 3 letters</div> : ''
    ];
  }

  render() {
    return (
      <div className={styles.fileDirectory}>
        <span><b>Search in file directory</b></span>
        <form className={classNames({ [styles.form]: true, [styles.focusedForm]: this.state.focused })}
          onSubmit={async (e) => {
            e ? e.preventDefault() : undefined;
            this.props.onSearch(this.validKeywords());
          }}>
          {this.renderNormalInput()}
        </form>
      </div>
    );
  }
}

export class ComponentDocuments extends React.Component {
  constructor(props) {
    super(props);
    this.onLink = this.onLink.bind(this);
  }

  areResultsAvailable(props) {
    return !!(props.data && props.data.values);
  }

  isFetching(props) {
    return !!(props && props.isFetching);
  }

  renderFilesHeader() {
    let code;
    let componentType = this.props.componentType.toLowerCase();
    switch (componentType) {
      case 'material':
        code = "Material Code";
        break;
      case 'service':
        code = "Service Code";
        break;
      case 'brand':
        code = 'Brand Code';
        break;
      case 'sfg':
        code = 'SFG Code';
        break;
      case 'package':
        code = 'Package Code';
        break;
    }
    return componentType === 'brand' ? (
      <thead>
        <tr className={styles.tableHeader}>
          <th className={styles.text}>Material Code</th>
          <th className={styles.text}>{code}</th>
          <th className={styles.text}>Manufacturer's Name</th>
          <th className={styles.text}></th>
        </tr>
      </thead>
    ) : (<thead>
      <tr className={styles.tableHeader}>
        <th className={styles.text}>Short Description</th>
        <th className={styles.text}>{code}</th>
        <th className={styles.text}>{this.props.columnHeader || this.props.columnName}</th>
      </tr>
    </thead>
      );
  }

  getDataItems() {
    return this.props.data && this.props.data.values && this.props.data.values.items;
  }

  onLink(e) {
    this.props.onLink(e);
  }

  renderFilesData() {
    switch (this.props.componentType.toLowerCase()) {
      case 'material':
        return this.getDataItems().map(dataItem => <ComponentDocument key={dataItem.materialCode}
          componentType={this.props.componentType}
          componentCode={dataItem.materialCode}
          shortDescription={dataItem.shortDescription}
          documents={dataItem.documentDtos}
          linkFile={this.props.linkFile}
          onLink={this.onLink} />);
      case 'service':
        return this.getDataItems().map(dataItem => <ComponentDocument key={dataItem.serviceCode}
          componentType={this.props.componentType}
          componentCode={dataItem.serviceCode}
          shortDescription={dataItem.shortDescription}
          documents={dataItem.documentDtos}
          linkFile={this.props.linkFile}
          onLink={this.onLink} />);
      case 'brand':
        return this.getDataItems().map(dataItem => <ComponentDocument key={dataItem.brandCode}
          componentType={this.props.componentType}
          componentCode={dataItem.materialCode}
          brandCode={dataItem.brandCode}
          manufacturersName={dataItem.manufacturersName}
          documents={dataItem.documentDtos}
          linkFile={this.props.linkFile}
          onLink={this.onLink} />);
      case 'package':
      case 'sfg':
        return this.getDataItems().map(dataItem => <ComponentDocument key={dataItem.code}
          componentType={this.props.componentType}
          componentCode={dataItem.code}
          shortDescription={dataItem.shortDescription}
          documents={dataItem.documentDtos}
          linkFile={this.props.linkFile}
          onLink={this.onLink} />);
    }
  }

  renderData() {
    let stylesList = [styles.table];
    if (this.props.componentType === 'brand') {
      stylesList.push(styles.brand);
    }
    if (this.getDataItems().length > 0) {
      return [
        <table key='table' className={stylesList.join(' ')}>
          {this.renderFilesHeader()}
          <tbody>
            {this.renderFilesData()}
          </tbody>
        </table>,
        <Pagination key="pagination" id={idFor('pagination')} current={this.props.data.values.pageNumber}
          total={this.props.data.values.recordCount}
          pageSize={this.props.data.values.batchSize} showSizeChanger={false}
          onChange={(pageNumber) => this.props.onPageNumberChange({ pageNumber })}
          locale={locale}
        />
      ];
    } else {
      return (
        <h3 className={styles.noResults}>No results are found</h3>
      );
    }
  }

  renderWaiting() {
    return this.props.error ? '' : <Loading />;
  }

  render() {
    return (
      <div>
        {this.areResultsAvailable(this.props) && !this.isFetching(this.props) ? this.renderData() : this.renderWaiting()}
      </div>
    );
  }
}

export class ComponentDocument extends React.Component {
  render() {
    let componentType = this.props.componentType && this.props.componentType.toLowerCase();
    return componentType === 'brand' ?
      (
        <tr key={this.props.brandCode} className={styles.tableRow}>
          <td className={styles.text}>{this.props.componentCode}</td>
          <td className={styles.text}>{this.props.brandCode}</td>
          <td className={styles.text}>{this.props.manufacturersName}</td>
          <td className={styles.text}>
            {this.props.documents.filter(d => d).map(document => <Document key={document.id} componentCode={this.props.componentCode}
              document={document} linkFile={this.props.linkFile}
              onLink={this.props.onLink} />)}
          </td>
        </tr>
      )
      :
      (
        <tr key={this.props.componentCode} className={styles.tableRow}>
          <td className={styles.text}>{this.props.shortDescription}</td>
          <td className={styles.text}>{this.props.componentCode}</td>
          <td className={styles.text}>
            {this.props.documents.filter(d => d).map(document => <Document key={document.id} componentCode={this.props.componentCode}
              document={document} linkFile={this.props.linkFile}
              onLink={this.props.onLink} />)}
          </td>
        </tr>
      );
  }
}

export class Document extends React.Component {
  render() {
    return (
      <div className={styles.document}>
        <a className={styles.link} target="_blank" href={this.props.document.url}>{this.props.document.name}</a>
        <button className={styles.linkFile} onClick={(e) => {
          e.preventDefault();
          this.props.onLink({ componentCode: this.props.componentCode, document: this.props.document });
        }}>
          {(this.props.linkFile.componentCode === this.props.componentCode && this.props.linkFile.document.id === this.props.document.id) ? 'Linked' : 'Link'}
        </button>
      </div>
    );
  }
}


